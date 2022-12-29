using System;
using System.Threading.Tasks;
using DSharpPlus.CommandAll.Commands.Enums;
using DSharpPlus.Entities;

namespace DSharpPlus.CommandAll.Commands.Converters
{
    /// <inheritdoc cref="IArgumentConverter{T}"/>
    public sealed class DiscordEmojiArgumentConverter : IArgumentConverter<DiscordEmoji>
    {
        /// <inheritdoc/>
        public ApplicationCommandOptionType OptionType => ApplicationCommandOptionType.String;

        /// <inheritdoc/>
        public ArgumentParsingBehavior ParsingBehavior => ArgumentParsingBehavior.Static;

        /// <inheritdoc/>
        public bool CanConvert(Type type) => type == typeof(DiscordEmoji);

        /// <inheritdoc/>
        public Task<Optional<DiscordEmoji>> ConvertAsync(CommandContext context, string value, CommandParameter? parameter = null)
            => Task.FromResult(DiscordEmoji.TryFromUnicode(context.Client, value, out DiscordEmoji? emoji) || DiscordEmoji.TryFromName(context.Client, value, out emoji)
                ? Optional.FromValue(emoji)
                : Optional.FromNoValue<DiscordEmoji>());
    }
}
