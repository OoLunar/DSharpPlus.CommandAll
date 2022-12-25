using System;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using DSharpPlus.CommandAll.Commands;
using DSharpPlus.CommandAll.Commands.Arguments;
using DSharpPlus.CommandAll.Commands.Enums;
using DSharpPlus.Entities;

namespace DSharpPlus.CommandAll.Converters
{
    public sealed partial class DiscordChannelArgumentConverter : IArgumentConverter<DiscordChannel>
    {
        public static ApplicationCommandOptionType OptionType { get; } = ApplicationCommandOptionType.Channel;

        [SuppressMessage("Roslyn", "IDE0046", Justification = "Silence the ternary rabbit hole.")]
        public Task<Optional<DiscordChannel>> ConvertAsync(CommandContext context, CommandParameter parameter, string value)
        {
            // Attempt to parse the channel id
            if (!ulong.TryParse(value, NumberStyles.None, CultureInfo.InvariantCulture, out ulong channelId))
            {
                // Value could be a channel mention.
                Match match = GetChannelRegex().Match(value);
                if (!match.Success || !ulong.TryParse(match.Groups[1].ValueSpan, NumberStyles.Number, CultureInfo.InvariantCulture, out channelId))
                {
                    // Attempt to find a channel by name, case insensitive.
                    DiscordChannel? namedChannel = context.Guild!.Channels.Values.FirstOrDefault(channel => channel.Name.Equals(value, StringComparison.OrdinalIgnoreCase));
                    return Task.FromResult(namedChannel is not null ? Optional.FromValue(namedChannel) : Optional.FromNoValue<DiscordChannel>());
                }
            }

            if (context.InvocationType == CommandInvocationType.SlashCommand && context.Interaction!.Data.Resolved?.Channels is not null && context.Interaction.Data.Resolved.Channels.TryGetValue(channelId, out DiscordChannel? channel))
            {
                return Task.FromResult(Optional.FromValue(channel));
            }
            else if (context.Guild!.GetChannel(channelId) is DiscordChannel guildChannel)
            {
                return Task.FromResult(Optional.FromValue(guildChannel));
            }
            else
            {
                return Task.FromResult(Optional.FromNoValue<DiscordChannel>());
            }
        }

        [GeneratedRegex(@"^<#(\d+)>$", RegexOptions.Compiled | RegexOptions.ECMAScript)]
        private static partial Regex GetChannelRegex();
    }
}
