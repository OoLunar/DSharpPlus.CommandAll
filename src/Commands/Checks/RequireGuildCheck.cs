using System.Threading;
using System.Threading.Tasks;

namespace DSharpPlus.CommandAll.Commands.Checks
{
    public class RequireGuildCheck : CommandCheckAttribute
    {
        public override Task<bool> CanExecuteAsync(CommandContext context, CancellationToken cancellationToken = default) => Task.FromResult(context.Guild is null);
    }
}
