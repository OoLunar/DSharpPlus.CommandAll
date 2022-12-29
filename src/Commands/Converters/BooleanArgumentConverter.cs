using System;
using System.Threading.Tasks;
using DSharpPlus.CommandAll.Commands.Enums;
using DSharpPlus.Entities;

namespace DSharpPlus.CommandAll.Commands.Converters
{
    /// <inheritdoc cref="IArgumentConverter{T}"/>
    public sealed class BooleanArgumentConverter : IArgumentConverter<bool>
    {
        /// <inheritdoc/>
        public ApplicationCommandOptionType OptionType => ApplicationCommandOptionType.Boolean;

        /// <inheritdoc/>
        public ArgumentParsingBehavior ParsingBehavior => ArgumentParsingBehavior.Static;

        /// <inheritdoc/>
        public bool CanConvert(Type type) => type == typeof(bool);

        /// <inheritdoc/>
        public Task<Optional<bool>> ConvertAsync(CommandContext context, string value, CommandParameter? parameter = null) => Task.FromResult(bool.TryParse(value, out bool result)
            ? Optional.FromValue(result)
            : Optional.FromNoValue<bool>());
    }
}
