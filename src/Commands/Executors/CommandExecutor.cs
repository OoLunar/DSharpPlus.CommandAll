using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

namespace OoLunar.DSharpPlus.CommandAll.Commands.Executors
{
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
        public virtual async Task<bool> ExecuteAsync(CommandContext context)
        {
            BaseCommand commandObject = (BaseCommand)ActivatorUtilities.CreateInstance(context.Extension.ServiceProvider, context.CurrentOverload.Method.DeclaringType!);
            Task task = Task.Run(async () =>
            {
                await commandObject.BeforeExecutionAsync(context);
                await (Task)context.CurrentOverload.Method.Invoke(commandObject, context.NamedArguments.Values.Prepend(context).ToArray())!;
                await commandObject.AfterExecutionAsync(context);
            });

            await task;

            if (!task.IsCompletedSuccessfully)
            {
                try
                {
                    await commandObject.OnErrorAsync(context, task.Exception!);
                }
                catch (Exception error)
                {
                    _logger.LogError(error, "An uncaught exception was thrown by {CommandName}'s {MethodName} method.", context.CurrentOverload.Method.DeclaringType!.Name, nameof(BaseCommand.OnErrorAsync));
                    return false;
                }
            }

            return true;
        }
    }
}
