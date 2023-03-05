using System.Threading.Tasks;

namespace DSharpPlus.CommandAll.Commands.Checks
{
    public class RequireNSFWCheck : CommandCheckAttribute
    {
        public CommandCheckAttribute GuildCheck { get; init; } = new RequireGuildCheck();

        public override async Task<bool> CanExecuteAsync(CommandContext context) => (await GuildCheck.CanExecuteAsync(context) && context.Guild!.NsfwLevel != NsfwLevel.Safe) || context.Channel.IsNSFW;
    }
}
