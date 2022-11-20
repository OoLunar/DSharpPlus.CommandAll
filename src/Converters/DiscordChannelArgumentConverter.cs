using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using DSharpPlus;
using DSharpPlus.Entities;
using OoLunar.DSharpPlus.CommandAll.Commands;
using OoLunar.DSharpPlus.CommandAll.Commands.Arguments;

namespace OoLunar.DSharpPlus.CommandAll.Converters
{
    public sealed partial class DiscordChannelArgumentConverter : IArgumentConverter<DiscordChannel>
    {
        public ApplicationCommandOptionType Type { get; init; } = ApplicationCommandOptionType.Channel;
        private static readonly Regex ChannelRegex = ChannelRegexMethod();

        [SuppressMessage("Roslyn", "IDE0046", Justification = "Silence the ternary rabbit hole.")]
        public Task<Optional<DiscordChannel>> ConvertAsync(CommandContext context, CommandParameter parameter, string value)
        {
            // value can be a raw channel id or a channel mention. The regex will match both.
            Match match = ChannelRegex.Match(value);
            if (!match.Success || !ulong.TryParse(match.Captures[0].ValueSpan, NumberStyles.Number, CultureInfo.InvariantCulture, out ulong channelId))
            {
                return Task.FromResult(Optional.FromNoValue<DiscordChannel>());
            }

            if (context.IsSlashCommand && context.Interaction!.Data.Resolved.Channels is not null && context.Interaction.Data.Resolved.Channels.TryGetValue(channelId, out DiscordChannel? channel))
            {
                return Task.FromResult(Optional.FromValue(channel));
            }

            channel = context.Guild!.GetChannel(channelId);
            return Task.FromResult(channel is not null ? Optional.FromValue(channel) : Optional.FromNoValue<DiscordChannel>());
        }

        [GeneratedRegex("^<#(\\d+)>$", RegexOptions.Compiled | RegexOptions.ECMAScript)]
        private static partial Regex ChannelRegexMethod();
    }
}
