using System;
using System.Diagnostics.CodeAnalysis;
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
    public sealed partial class DiscordUserArgumentConverter : IArgumentConverter<DiscordUser>
    {
        public ApplicationCommandOptionType OptionType { get; init; } = ApplicationCommandOptionType.User;
        private readonly ILogger<DiscordUserArgumentConverter> _logger;

        public DiscordUserArgumentConverter(ILogger<DiscordUserArgumentConverter> logger) => _logger = logger ?? throw new ArgumentNullException(nameof(logger));

        [SuppressMessage("Roslyn", "IDE0046", Justification = "Silence the ternary rabbit hole.")]
        public async Task<Optional<DiscordUser>> ConvertAsync(CommandContext context, string value, CommandParameter? parameter = null)
        {
            if (!ulong.TryParse(value, NumberStyles.Number, CultureInfo.InvariantCulture, out ulong memberId))
            {
                Match match = GetMemberRegex().Match(value);
                if (!match.Success || !ulong.TryParse(match.Captures[0].ValueSpan, NumberStyles.Number, CultureInfo.InvariantCulture, out memberId))
                {
                    // Attempt to find a member by name, case insensitive.
                    DiscordUser? namedMember = context.Guild!.Members.Values.FirstOrDefault(member => member.DisplayName.Equals(value, StringComparison.OrdinalIgnoreCase));
                    return namedMember is not null ? Optional.FromValue(namedMember) : Optional.FromNoValue<DiscordUser>();
                }
            }

            // Attempt to use the resolved members/users from the interaction.
            if (context.InvocationType == CommandInvocationType.SlashCommand)
            {
                // Always attempt to pass the member before falling back on the user in case the dev wants to cast to a member.
                if (context.Interaction!.Data.Resolved?.Members is not null && context.Interaction.Data.Resolved.Members.TryGetValue(memberId, out DiscordMember? member))
                {
                    return Optional.FromValue(member as DiscordUser);
                }
                else if (context.Interaction.Data.Resolved?.Users is not null && context.Interaction.Data.Resolved.Users.TryGetValue(memberId, out DiscordUser? user))
                {
                    return Optional.FromValue(user);
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

        [GeneratedRegex(@"^<@\!?(\d+?)>$", RegexOptions.Compiled | RegexOptions.ECMAScript)]
        private static partial Regex GetMemberRegex();
    }
}
