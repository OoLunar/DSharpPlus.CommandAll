using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
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
        public virtual async Task<bool> ExecuteAsync(CommandContext context)
        {
            if (context.CurrentOverload.Flags.HasFlag(CommandOverloadFlags.Disabled))
            {
                await ExecuteErrorHandlerAsync(context, null!, new CommandDisabledException(context.CurrentCommand));
                return false;
            }

            ConcurrentBag<CommandCheckResult> checkStatuses = new();
            CancellationTokenSource cancellationTokenSource = new();
            await Parallel.ForEachAsync(context.CurrentOverload.Checks, cancellationTokenSource.Token, async (check, cancellationToken) =>
            {
                try
                {
                    _logger.LogTrace("{CommandName}: Executing check {CheckName}.", context.CurrentCommand.Name, check.GetType());
                    if (!await check.CanExecuteAsync(context, cancellationTokenSource.Token))
                    {
                        _logger.LogDebug("{CommandName}: Check {CheckName} failed.", context.CurrentCommand.Name, check.GetType());
                        cancellationTokenSource.Cancel(false);
                        checkStatuses.Add(new CommandCheckResult(check, false));
                    }

                    checkStatuses.Add(new CommandCheckResult(check, true));
                }
                // A different check had failed and now we're cancelling the rest
                catch (TaskCanceledException) when (cancellationToken.IsCancellationRequested)
                {
                    checkStatuses.Add(new CommandCheckResult(check, false));
                }
                // A check threw an exception, catch it and mark it as failed. Cancel the other checks too.
                catch (Exception error)
                {
                    _logger.LogError(error, "{CommandName}: Check {CheckName} threw an exception.", context.CurrentCommand.Name, check.GetType());
                    cancellationTokenSource.Cancel(false);
                    checkStatuses.Add(new CommandCheckResult(check, false, error));
                }
            });

            // If any checks fail, skip the command execution and run the error handler with the CommandChecksFailedException
            if (checkStatuses.Any(x => !x.Success))
            {
                await ExecuteErrorHandlerAsync(context, null!, new CommandChecksFailedException($"One or more checks failed when attempting to execute command {context.CurrentCommand.Name}.", checkStatuses.ToList()));
                return false;
            }

            BaseCommand? commandObject = null;
            List<object> tasks = new();
            if (context.CurrentOverload.Flags.HasFlag(CommandOverloadFlags.RequireObject))
            {
                // Use DI to create the object.
                commandObject = (BaseCommand)ActivatorUtilities.CreateInstance(context.Extension.ServiceProvider, context.CurrentCommand.Type);

                tasks.Add(commandObject.BeforeExecutionAsync(context));
                tasks.Add(context.CurrentOverload.Method.Invoke(commandObject, context.NamedArguments.Values.Prepend(context).ToArray())!);
                tasks.Add(commandObject.AfterExecutionAsync(context));
            }
            else
            {
                tasks.Add(context.CurrentOverload.Method.Invoke(null, context.NamedArguments.Values.Prepend(context).ToArray())!);
            }

            foreach (object task in tasks)
            {
                try
                {
                    if (task is Task t)
                    {
                        await t;
                    }
                    else if (task is ValueTask vt)
                    {
                        await vt;
                    }
                    else
                    {
                        // TODO: Should throw on startup during command building.
                        throw new InvalidOperationException($"{context.CurrentOverload.Method.DeclaringType!.FullName}.{context.CurrentOverload.Method.Name} does not return a Task or ValueTask.");
                    }
                }
                catch (TargetInvocationException targetError)
                {
                    await ExecuteErrorHandlerAsync(context, commandObject, targetError.InnerException!);
                    return false;
                }
                catch (Exception error)
                {
                    await ExecuteErrorHandlerAsync(context, commandObject, error);
                    return false;
                }
            }

            return true;
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
