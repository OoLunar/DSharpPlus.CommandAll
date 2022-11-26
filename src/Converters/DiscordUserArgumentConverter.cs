using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.Exceptions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using OoLunar.DSharpPlus.CommandAll.Commands;
using OoLunar.DSharpPlus.CommandAll.Commands.Arguments;

namespace OoLunar.DSharpPlus.CommandAll.Converters
{
    public sealed partial class DiscordUserArgumentConverter : IArgumentConverter<DiscordUser>
    {
        public static ApplicationCommandOptionType OptionType { get; } = ApplicationCommandOptionType.String;
        private static readonly Regex MemberRegex = MemberRegexMethod();
        private readonly ILogger<DiscordUserArgumentConverter> _logger;

        public DiscordUserArgumentConverter(ILogger<DiscordUserArgumentConverter> logger) => _logger = logger ?? NullLogger<DiscordUserArgumentConverter>.Instance;

        [SuppressMessage("Roslyn", "IDE0046", Justification = "Silence the ternary rabbit hole.")]
        public async Task<Optional<DiscordUser>> ConvertAsync(CommandContext context, CommandParameter parameter, string value)
        {
            Match match = MemberRegex.Match(value);
            if (!match.Success || !ulong.TryParse(match.Captures[0].ValueSpan, NumberStyles.Number, CultureInfo.InvariantCulture, out ulong memberId))
            {
                return Optional.FromNoValue<DiscordUser>();
            }

            // Attempt to use the resolved members/users from the interaction.
            if (context.IsSlashCommand)
            {
                // Always attempt to pass the member before falling back on the user in case the dev wants to cast to a member.
                if (context.Interaction!.Data.Resolved.Members != null && context.Interaction.Data.Resolved.Members.TryGetValue(memberId, out DiscordMember? member))
                {
                    return Optional.FromValue(member as DiscordUser);
                }
                else if (context.Interaction.Data.Resolved.Users != null && context.Interaction.Data.Resolved.Users.TryGetValue(memberId, out DiscordUser? user))
                {
                    return Optional.FromValue(user);
                }
            }

            // Iterate through all the guilds on the shard to find the user.
            // We could iterate through all the shards, however I don't know how long it would take (nor do I have a way to measure it)
            foreach (DiscordGuild guild in context.Client.Guilds.Values)
            {
                if (guild.Members.TryGetValue(memberId, out DiscordMember? member))
                {
                    return Optional.FromValue(member as DiscordUser);
                }
            }

            // If we didn't find the user in any guild, try to get the user from the API.
            try
            {
                DiscordUser? possiblyCachedUser = await context.Client.GetUserAsync(memberId);
                if (possiblyCachedUser is not null)
                {
                    return Optional.FromValue(possiblyCachedUser);
                }
            }
            catch (DiscordException error)
            {
                _logger.LogError(error, "Failed to get user from client.");
            }

            return Optional.FromNoValue<DiscordUser>();
        }

        [GeneratedRegex(@"^<@\\!?(\d+?)>$", RegexOptions.Compiled | RegexOptions.ECMAScript)]
        private static partial Regex MemberRegexMethod();
    }
}
