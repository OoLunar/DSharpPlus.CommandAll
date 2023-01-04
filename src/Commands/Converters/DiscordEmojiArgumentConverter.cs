using System.Threading.Tasks;
using DSharpPlus.CommandAll.Commands.Enums;
using DSharpPlus.Entities;

namespace DSharpPlus.CommandAll.Commands.Converters
{
    /// <inheritdoc cref="ArgumentConverter{T}"/>
    public sealed class DiscordEmojiArgumentConverter : ArgumentConverter<DiscordEmoji>
    {
        /// <inheritdoc/>
        public override ApplicationCommandOptionType OptionType => ApplicationCommandOptionType.String;

        /// <inheritdoc/>
        public override ArgumentParsingBehavior ParsingBehavior => ArgumentParsingBehavior.Static;

        /// <inheritdoc/>
        public override Task<Optional<DiscordEmoji>> ConvertAsync(CommandContext context, string value, CommandParameter? parameter = null)
            => Task.FromResult(DiscordEmoji.TryFromUnicode(context.Client, value, out DiscordEmoji? emoji) || DiscordEmoji.TryFromName(context.Client, value, out emoji)
                ? Optional.FromValue(emoji)
                : Optional.FromNoValue<DiscordEmoji>());
    }
}
