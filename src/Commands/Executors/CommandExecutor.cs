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
            // Constructor DI only
            BaseCommand commandObject = (BaseCommand)ActivatorUtilities.CreateInstance(context.Extension.ServiceProvider, context.CurrentOverload.Method.DeclaringType!);
            try
            {
                _logger.LogTrace("{CommandName}: Executing before execution check.", context.CurrentCommand.Name);
                await commandObject.BeforeExecutionAsync(context);
                _logger.LogDebug("{CommandName}: Executing command with {Arguments}...", context.CurrentCommand.Name, string.Join(", ", context.NamedArguments.Select(x => x.ToString())));
                _logger.LogTrace("{CommandName}: Executing command overload {OverloadName}...", context.CurrentCommand.Name, context.CurrentOverload.Method);
                await (Task)context.CurrentOverload.Method.Invoke(commandObject, context.NamedArguments.Values.Prepend(context).ToArray())!;
                _logger.LogTrace("{CommandName}: Executing after command async.", context.CurrentCommand.Name);
                await commandObject.AfterExecutionAsync(context);
            }
            catch (Exception error)
            {
                try
                {
                    _logger.LogTrace("{CommandName}: Executing error handler...", context.CurrentCommand.Name);
                    await commandObject.OnErrorAsync(context, error);
                }
                catch (Exception error2)
                {
                    _logger.LogError(error2, "An uncaught exception was thrown by {CommandName}'s error handler.", context.CurrentCommand.Name);
                    return false;
                }
            }

            _logger.LogDebug("{CommandName}: Command executed successfully.", context.CurrentCommand.Name);
            return true;
        }
    }
}
