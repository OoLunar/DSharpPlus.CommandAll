using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using DSharpPlus;
using DSharpPlus.Entities;
using OoLunar.DSharpPlus.CommandAll.Commands;
using OoLunar.DSharpPlus.CommandAll.Commands.Arguments;
using OoLunar.DSharpPlus.CommandAll.Commands.System.Commands;

namespace OoLunar.DSharpPlus.CommandAll.Converters
{
    public sealed partial class DiscordThreadChannelArgumentConverter : IArgumentConverter<DiscordThreadChannel>
    {
        public static ApplicationCommandOptionType OptionType { get; } = ApplicationCommandOptionType.Channel;

        [SuppressMessage("Roslyn", "IDE0046", Justification = "Silence the ternary rabbit hole.")]
        public Task<Optional<DiscordThreadChannel>> ConvertAsync(CommandContext context, CommandParameter parameter, string value)
        {
            if (!ulong.TryParse(value, NumberStyles.Number, CultureInfo.InvariantCulture, out ulong channelId))
            {
                // value can be a raw channel id or a channel mention. The regex will match both.
                Match match = GetChannelRegex().Match(value);
                if (!match.Success || !ulong.TryParse(match.Captures[0].ValueSpan, NumberStyles.Number, CultureInfo.InvariantCulture, out channelId))
                {
                    return Task.FromResult(Optional.FromNoValue<DiscordThreadChannel>());
                }
            }

            if (context.IsSlashCommand && context.Interaction!.Data.Resolved.Channels is not null && context.Interaction.Data.Resolved.Channels.TryGetValue(channelId, out DiscordChannel? channel) && channel is DiscordThreadChannel threadChannel)
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
