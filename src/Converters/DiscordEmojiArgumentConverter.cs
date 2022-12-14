using System.Threading.Tasks;
using DSharpPlus.CommandAll.Commands;
using DSharpPlus.CommandAll.Commands.Arguments;
using DSharpPlus.Entities;

namespace DSharpPlus.CommandAll.Converters
{
    public sealed partial class DiscordEmojiArgumentConverter : IArgumentConverter<DiscordEmoji>
    {
        public static ApplicationCommandOptionType OptionType { get; } = ApplicationCommandOptionType.String;

        public Task<Optional<DiscordEmoji>> ConvertAsync(CommandContext context, CommandParameter parameter, string value)
        => Task.FromResult(DiscordEmoji.TryFromUnicode(context.Client, value, out DiscordEmoji? emoji) || DiscordEmoji.TryFromName(context.Client, value, out emoji)
            ? Optional.FromValue(emoji)
            : Optional.FromNoValue<DiscordEmoji>());
    }
}
