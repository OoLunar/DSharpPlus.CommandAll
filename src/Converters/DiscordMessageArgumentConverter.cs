using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.Exceptions;
using OoLunar.DSharpPlus.CommandAll.Commands;
using OoLunar.DSharpPlus.CommandAll.Commands.Arguments;
using OoLunar.DSharpPlus.CommandAll.Commands.System.Commands;

namespace OoLunar.DSharpPlus.CommandAll.Converters
{
    public sealed partial class DiscordMessageArgumentConverter : IArgumentConverter<DiscordMessage>
    {
        public static ApplicationCommandOptionType OptionType { get; } = ApplicationCommandOptionType.String;

        [SuppressMessage("Roslyn", "IDE0046", Justification = "Silence the ternary rabbit hole.")]
        public async Task<Optional<DiscordMessage>> ConvertAsync(CommandContext context, CommandParameter parameter, string value)
        {
            Match match = GetMessageRegex().Match(value);
            if (!match.Success || !ulong.TryParse(match.Groups["message"].ValueSpan, NumberStyles.Number, CultureInfo.InvariantCulture, out ulong messageId))
            {
                return Optional.FromNoValue<DiscordMessage>();
            }

            if (context.IsSlashCommand && context.Interaction!.Data.Resolved?.Messages is not null && context.Interaction.Data.Resolved.Messages.TryGetValue(messageId, out DiscordMessage? message))
            {
                return Optional.FromValue(message);
            }

            if (!match.Groups.TryGetValue("channel", out Group? channelGroup) || !ulong.TryParse(channelGroup.ValueSpan, NumberStyles.Number, CultureInfo.InvariantCulture, out ulong channelId))
            {
                return Optional.FromNoValue<DiscordMessage>();
            }

            DiscordChannel? channel = null;
            if (match.Groups.TryGetValue("guild", out Group? guildGroup) && ulong.TryParse(guildGroup.ValueSpan, NumberStyles.Number, CultureInfo.InvariantCulture, out ulong guildId) && context.Client.Guilds.TryGetValue(guildId, out DiscordGuild? guild))
            {
                // Make sure the message belongs to the guild
                if (guild.Id != context.Guild!.Id)
                {
                    return Optional.FromNoValue<DiscordMessage>();
                }
                else if (guild.Channels.TryGetValue(channelId, out DiscordChannel? guildChannel))
                {
                    channel = guildChannel;
                }
                // guildGroup is null which means the link used @me, which means DM's. At this point, we can only get the message if the DM is with the bot.
                else if (guildGroup is null && channelId == context.Client.CurrentUser.Id)
                {
                    channel = context.Client.PrivateChannels.TryGetValue(context.User.Id, out DiscordDmChannel? dmChannel) ? dmChannel : null;
                }
            }


            if (channel is null)
            {
                return Optional.FromNoValue<DiscordMessage>();
            }

            try
            {
                message = await channel.GetMessageAsync(messageId);
            }
            catch (DiscordException)
            {
                // Not logging because users can intentionally give us incorrect data to intentionally spam logs.
                message = null;
            }
            return message is not null ? Optional.FromValue(message) : Optional.FromNoValue<DiscordMessage>();
        }

        [GeneratedRegex(@"\/channels\/(?<guild>(?:\d+|@me))\/(?<channel>\d+)\/(?<message>\d+)\/?", RegexOptions.Compiled | RegexOptions.ECMAScript)]
        private static partial Regex GetMessageRegex();
    }
}
