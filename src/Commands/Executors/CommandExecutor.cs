using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using OoLunar.DSharpPlus.CommandAll.Commands.Enums;
using OoLunar.DSharpPlus.CommandAll.EventArgs;

namespace OoLunar.DSharpPlus.CommandAll.Commands.Executors
{
    /// <inheritdoc cref="ICommandExecutor"/>
    public class CommandExecutor : ICommandExecutor
    {
        /// <summary>
        /// Allows logging of exceptions caused by <see cref="BaseCommand.OnErrorAsync(CommandContext, Exception)"/>.
        /// </summary>
        private readonly ILogger<CommandExecutor> _logger = NullLogger<CommandExecutor>.Instance;

        /// <summary>
        /// Creates a new instance of the <see cref="CommandExecutor"/> class, which runs commands through <see cref="Task.Run(Action, CancellationToken)"/>.
        /// </summary>
        /// <param name="logger">Which logger to use when reporting uncaught exceptions.</param>
        public CommandExecutor(ILogger<CommandExecutor>? logger = null) => _logger = logger ?? NullLogger<CommandExecutor>.Instance;

        /// <summary>
        /// Executes a command asynchronously through <see cref="Task.Run(Action, CancellationToken)"/>.
        /// </summary>
        /// <param name="context">The context for the command.</param>
        /// <returns>Whether the command executed successfully without any uncaught exceptions.</returns>
        public virtual Task<bool> ExecuteAsync(CommandContext context)
        {
            if (context.CurrentOverload.Flags.HasFlag(CommandOverloadFlags.Disabled))
            {
                return Task.FromResult(false);
            }

            _ = Task.Run(async () =>
            {
                // Constructor DI only
                BaseCommand commandObject = (BaseCommand)ActivatorUtilities.CreateInstance(context.Extension.ServiceProvider, context.CurrentOverload.Method.DeclaringType!);

                _logger.LogTrace("{CommandName}: Executing before execution check.", context.CurrentCommand.Name);
                Task beforeTask = commandObject.BeforeExecutionAsync(context);
                await beforeTask;
                if (!beforeTask.IsCompletedSuccessfully)
                {
                    return await ExecuteErrorHandlerAsync(context, commandObject, beforeTask.Exception!.InnerExceptions[0]);
                }

                _logger.LogDebug("{CommandName}: Executing command overload {Overload} with {Arguments}...", context.CurrentCommand.Name, context.CurrentOverload.Method, string.Join(", ", context.NamedArguments.Values.Select(x => x?.ToString())));
                Task commandTask = (Task)context.CurrentOverload.Method.Invoke(commandObject, context.NamedArguments.Values.Prepend(context).ToArray())!;
                await commandTask;
                if (!commandTask.IsCompletedSuccessfully)
                {
                    return await ExecuteErrorHandlerAsync(context, commandObject, commandTask.Exception!.InnerExceptions[0]);
                }

                _logger.LogTrace("{CommandName}: Executing after command async.", context.CurrentCommand.Name);
                Task afterTask = commandObject.AfterExecutionAsync(context);
                await afterTask;
                if (!afterTask.IsCompletedSuccessfully)
                {
                    return await ExecuteErrorHandlerAsync(context, commandObject, afterTask.Exception!.InnerExceptions[0]);
                }

                _logger.LogDebug("{CommandName}: Command executed successfully.", context.CurrentCommand.Name);
                return true;
            });

            return Task.FromResult(true);
        }

        /// <summary>
        /// Attempts to execute <see cref="BaseCommand.OnErrorAsync(CommandContext, Exception)"/>. If that method is not implemented or throws an exception, <see cref="CommandAllExtension._commandErrored"/> is invoked. If that fails, then exception is logged.
        /// </summary>
        private async Task<bool> ExecuteErrorHandlerAsync(CommandContext context, BaseCommand commandObject, Exception error)
        {
            _logger.LogTrace("{CommandName}: Executing error handler...", context.CurrentCommand.Name);
            Task errorTask = commandObject.OnErrorAsync(context, error);
            await errorTask;
            if (!errorTask.IsCompletedSuccessfully)
            {
                // Not Implemented Exception is the default exception thrown by the error handler when it is not overridden.
                // Because we don't want to mess up the stack trace, we're explicitly checking for that exception
                // And returning the error parameter instead.
                if (errorTask.Exception!.InnerException is NotImplementedException)
                {
                    await context.Extension._commandErrored.InvokeAsync(context.Extension, new CommandErroredEventArgs(context, error));
                }
                else
                {
                    _logger.LogError(errorTask.Exception, "{CommandName}: Error handler threw an exception.", context.CurrentCommand.Name);
                    await context.Extension._commandErrored.InvokeAsync(context.Extension, new CommandErroredEventArgs(context, errorTask.Exception!.InnerException!));
                }
                return false;
            }

            return true;
        }
    }
}
