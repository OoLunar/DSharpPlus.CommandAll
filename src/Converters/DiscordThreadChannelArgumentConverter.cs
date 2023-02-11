using System;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using DSharpPlus.CommandAll.Commands;
using DSharpPlus.CommandAll.Commands.Enums;
using DSharpPlus.Entities;

namespace DSharpPlus.CommandAll.Converters
{
    public sealed partial class DiscordThreadChannelArgumentConverter : IArgumentConverter<DiscordThreadChannel>
    {
        public ApplicationCommandOptionType OptionType { get; init; } = ApplicationCommandOptionType.Channel;

        [SuppressMessage("Roslyn", "IDE0046", Justification = "Silence the ternary rabbit hole.")]
        public Task<Optional<DiscordThreadChannel>> ConvertAsync(CommandContext context, string value, CommandParameter? parameter = null)
        {
            if (!ulong.TryParse(value, NumberStyles.Number, CultureInfo.InvariantCulture, out ulong channelId))
            {
                // value can be a raw channel id or a channel mention. The regex will match both.
                Match match = GetChannelRegex().Match(value);
                if (!match.Success || !ulong.TryParse(match.Captures[0].ValueSpan, NumberStyles.Number, CultureInfo.InvariantCulture, out channelId))
                {
                    // Attempt to find a thread channel by name, case insensitive.
                    DiscordThreadChannel? namedChannel = context.Guild!.Threads.Values.FirstOrDefault(channel => channel.Name.Equals(value, StringComparison.OrdinalIgnoreCase));
                    return Task.FromResult(namedChannel is not null ? Optional.FromValue(namedChannel) : Optional.FromNoValue<DiscordThreadChannel>());
                }
            }

            if (context.InvocationType == CommandInvocationType.SlashCommand && context.Interaction!.Data.Resolved?.Channels is not null && context.Interaction.Data.Resolved.Channels.TryGetValue(channelId, out DiscordChannel? channel) && channel is DiscordThreadChannel threadChannel)
            {
                return Task.FromResult(Optional.FromValue(threadChannel));
            }

            // Not merged with the previous if statement for readability.
            if (context.Guild!.Threads.TryGetValue(channelId, out threadChannel!) && threadChannel is not null)
            {
                return Task.FromResult(Optional.FromValue(threadChannel));
            }
            else
            {
                return Task.FromResult(Optional.FromNoValue<DiscordThreadChannel>());
            }
        }

        [GeneratedRegex(@"^<#(\d+)>$", RegexOptions.Compiled | RegexOptions.ECMAScript)]
        private static partial Regex GetChannelRegex();
    }
}
