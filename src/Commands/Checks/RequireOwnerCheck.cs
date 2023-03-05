using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace DSharpPlus.CommandAll.Commands.Checks
{
    public class RequireOwnerCheck : CommandCheckAttribute
    {
        public override Task<bool> CanExecuteAsync(CommandContext context, CancellationToken cancellationToken = default) => Task.FromResult(context.Client.CurrentApplication.Owners.Contains(context.User));
    }
}
