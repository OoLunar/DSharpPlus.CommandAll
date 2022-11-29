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
using OoLunar.DSharpPlus.CommandAll.Commands.System.Commands;

namespace OoLunar.DSharpPlus.CommandAll.Converters
{
    public sealed partial class DiscordMemberArgumentConverter : IArgumentConverter<DiscordMember>
    {
        public static ApplicationCommandOptionType OptionType { get; } = ApplicationCommandOptionType.User;
        private readonly ILogger<DiscordMemberArgumentConverter> _logger;

        public DiscordMemberArgumentConverter(ILogger<DiscordMemberArgumentConverter> logger) => _logger = logger ?? NullLogger<DiscordMemberArgumentConverter>.Instance;

        public async Task<Optional<DiscordMember>> ConvertAsync(CommandContext context, CommandParameter parameter, string value)
        {
            if (!ulong.TryParse(value, NumberStyles.Number, CultureInfo.InvariantCulture, out ulong memberId))
            {
                Match match = GetMemberRegex().Match(value);
                if (!match.Success || !ulong.TryParse(match.Captures[0].ValueSpan, NumberStyles.Number, CultureInfo.InvariantCulture, out memberId))
                {
                    return Optional.FromNoValue<DiscordMember>();
                }
            }

            if (context.IsSlashCommand && context.Interaction!.Data.Resolved.Members != null && context.Interaction.Data.Resolved.Members.TryGetValue(memberId, out DiscordMember? member))
            {
                return Optional.FromValue(member);
            }

            try
            {
                DiscordMember? possiblyCachedMember = await context.Guild!.GetMemberAsync(memberId);
                return possiblyCachedMember is not null ? Optional.FromValue(possiblyCachedMember) : Optional.FromNoValue<DiscordMember>();
            }
            catch (DiscordException error)
            {
                _logger.LogError(error, "Failed to get member from guild.");
                return Optional.FromNoValue<DiscordMember>();
            }
        }

        [GeneratedRegex(@"^<@\!?(\d+?)>$", RegexOptions.Compiled | RegexOptions.ECMAScript)]
        private static partial Regex GetMemberRegex();
    }
}
