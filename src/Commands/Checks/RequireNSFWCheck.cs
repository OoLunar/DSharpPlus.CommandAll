using System.Threading;
using System.Threading.Tasks;

namespace DSharpPlus.CommandAll.Commands.Checks
{
    public class RequireNSFWCheck : CommandCheckAttribute
    {
        public CommandCheckAttribute GuildCheck { get; init; } = new RequireGuildCheck();

        public override async Task<bool> CanExecuteAsync(CommandContext context, CancellationToken cancellationToken = default) => (await GuildCheck.CanExecuteAsync(context) && context.Guild!.NsfwLevel != NsfwLevel.Safe) || context.Channel.IsNSFW;
    }
}
