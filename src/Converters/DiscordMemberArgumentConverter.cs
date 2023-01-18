using System;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using DSharpPlus.CommandAll.Commands;
using DSharpPlus.CommandAll.Commands.Enums;
using DSharpPlus.Entities;
using DSharpPlus.Exceptions;
using Microsoft.Extensions.Logging;

namespace DSharpPlus.CommandAll.Converters
{
    public sealed partial class DiscordMemberArgumentConverter : IArgumentConverter<DiscordMember>
    {
        public static ApplicationCommandOptionType OptionType { get; } = ApplicationCommandOptionType.User;
        private readonly ILogger<DiscordMemberArgumentConverter> _logger;

        public DiscordMemberArgumentConverter(ILogger<DiscordMemberArgumentConverter> logger) => _logger = logger ?? throw new ArgumentNullException(nameof(logger));

        public async Task<Optional<DiscordMember>> ConvertAsync(CommandContext context, CommandParameter parameter, string value)
        {
            if (!ulong.TryParse(value, NumberStyles.Number, CultureInfo.InvariantCulture, out ulong memberId))
            {
                Match match = GetMemberRegex().Match(value);
                if (!match.Success || !ulong.TryParse(match.Captures[0].ValueSpan, NumberStyles.Number, CultureInfo.InvariantCulture, out memberId))
                {
                    // Attempt to find a member by name, case insensitive.
                    DiscordMember? namedMember = context.Guild!.Members.Values.FirstOrDefault(member => member.DisplayName.Equals(value, StringComparison.OrdinalIgnoreCase));
                    return namedMember is not null ? Optional.FromValue(namedMember) : Optional.FromNoValue<DiscordMember>();
                }
            }

            if (context.InvocationType == CommandInvocationType.SlashCommand && context.Interaction!.Data.Resolved?.Members is not null && context.Interaction.Data.Resolved.Members.TryGetValue(memberId, out DiscordMember? member))
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
