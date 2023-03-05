using System;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using DSharpPlus.CommandAll.Commands.Enums;
using DSharpPlus.CommandAll.EventArgs;
using DSharpPlus.CommandAll.Exceptions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace DSharpPlus.CommandAll.Commands.Executors
{
    /// <inheritdoc cref="ICommandExecutor"/>
    public class CommandExecutor : ICommandExecutor
    {
        /// <summary>
        /// Allows logging of exceptions caused by <see cref="BaseCommand.OnErrorAsync(CommandContext, Exception)"/>.
        /// </summary>
        private readonly ILogger<CommandExecutor> _logger;

        /// <summary>
        /// Creates a new instance of the <see cref="CommandExecutor"/> class, which runs commands through <see cref="Task.Run(Action, CancellationToken)"/>.
        /// </summary>
        /// <param name="logger">Which logger to use when reporting uncaught exceptions.</param>
        public CommandExecutor(ILogger<CommandExecutor>? logger = null) => _logger = logger ?? throw new ArgumentNullException(nameof(logger));

        /// <summary>
        /// Executes a command asynchronously through <see cref="Task.Run(Action, CancellationToken)"/>.
        /// </summary>
        /// <param name="context">The context for the command.</param>
        /// <returns>Whether the command executed successfully without any uncaught exceptions.</returns>
        public virtual Task<bool> ExecuteAsync(CommandContext context)
        {
            if (context.CurrentOverload.Flags.HasFlag(CommandOverloadFlags.Disabled))
            {
                return ExecuteErrorHandlerAsync(context, null!, new CommandDisabledException(context.CurrentCommand)).ContinueWith(task => false);
            }

            _ = Task.Run(async () =>
            {
                // Constructor DI only
                BaseCommand commandObject = (BaseCommand)ActivatorUtilities.CreateInstance(context.ServiceProvider, context.CurrentOverload.Method.DeclaringType!);
                Task task;

                try
                {
                    CancellationTokenSource cancellationTokenSource = new();
                    await Parallel.ForEachAsync(context.CurrentOverload.Checks, cancellationTokenSource.Token, async (check, cancellationToken) =>
                    {
                        _logger.LogTrace("{CommandName}: Executing check {CheckName}.", context.CurrentCommand.Name, check.GetType());
                        if (!await check.CanExecuteAsync(context, cancellationToken))
                        {
                            _logger.LogDebug("{CommandName}: Check {CheckName} failed.", context.CurrentCommand.Name, check.GetType());
                            cancellationTokenSource.Cancel(false);
                        }
                    });

                    if (cancellationTokenSource.IsCancellationRequested)
                    {
                        return false;
                    }

                    _logger.LogTrace("{CommandName}: Executing before execution check.", context.CurrentCommand.Name);
                    task = commandObject.BeforeExecutionAsync(context);
                    await task;

                    _logger.LogDebug("{CommandName}: Executing command overload {Overload} with {Arguments}...", context.CurrentCommand.Name, context.CurrentOverload.Method, string.Join(", ", context.NamedArguments.Values.Select(x => x?.ToString())));
                    object? result = context.CurrentOverload.Method.Invoke(commandObject, context.NamedArguments.Values.Prepend(context).ToArray());
                    if (result is Task commandTask)
                    {
                        task = commandTask;
                        await task;
                    }

                    _logger.LogTrace("{CommandName}: Executing after command async.", context.CurrentCommand.Name);
                    task = commandObject.AfterExecutionAsync(context);
                    await task;
                }
                catch (TargetInvocationException targetError)
                {
                    _logger.LogTrace("{CommandName}: Executing error handler async.", context.CurrentCommand.Name);
                    await ExecuteErrorHandlerAsync(context, commandObject, targetError.InnerException!);
                    return false;
                }
                catch (Exception error)
                {
                    _logger.LogTrace("{CommandName}: Executing error handler async.", context.CurrentCommand.Name);
                    await ExecuteErrorHandlerAsync(context, commandObject, error);
                    return false;
                }

                _logger.LogDebug("{CommandName}: Command executed successfully.", context.CurrentCommand.Name);
                return true;
            });

            return Task.FromResult(true);
        }

        /// <summary>
        /// Attempts to execute <see cref="BaseCommand.OnErrorAsync(CommandContext, Exception)"/>. If that method is not implemented or throws an exception, <see cref="CommandAllExtension._commandErrored"/> is invoked. If that fails, then exception is logged.
        /// </summary>
        private async Task ExecuteErrorHandlerAsync(CommandContext context, BaseCommand? commandObject, Exception error)
        {
            if (commandObject is null)
            {
                await context.Extension._commandErrored.InvokeAsync(context.Extension, new CommandErroredEventArgs(context, error));
                return;
            }

            try
            {
                await commandObject.OnErrorAsync(context, error);
            }
            catch (HandlerNotImplementedException)
            {
                await context.Extension._commandErrored.InvokeAsync(context.Extension, new CommandErroredEventArgs(context, error));
            }
            catch (Exception otherError)
            {
                _logger.LogError(otherError, "{CommandName}: Error handler threw an exception.", context.CurrentCommand.Name);
                await context.Extension._commandErrored.InvokeAsync(context.Extension, new CommandErroredEventArgs(context, otherError));
            }
        }
    }
}
