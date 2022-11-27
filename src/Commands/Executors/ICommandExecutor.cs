using System.Threading.Tasks;

namespace OoLunar.DSharpPlus.CommandAll.Commands.Executors
{
    /// <summary>
    /// It executes commands. :/
    /// </summary>
    public interface ICommandExecutor
    {
        /// <summary>
        /// Executes a command.
        /// </summary>
        /// <param name="context">The context for the command.</param>
        /// <returns>Whether the command executed successfully without any uncaught exceptions.</returns>
        Task<bool> ExecuteAsync(CommandContext context);
    }
}
