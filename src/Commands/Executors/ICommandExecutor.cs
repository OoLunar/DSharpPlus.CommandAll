using System.Threading.Tasks;

namespace OoLunar.DSharpPlus.CommandAll.Commands.Executors
{
    public interface ICommandExecutor
    {
        Task<bool> ExecuteAsync(CommandContext context);
    }
}
