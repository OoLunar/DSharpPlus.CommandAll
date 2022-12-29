using System;
using System.Threading.Tasks;
using DSharpPlus.CommandAll.Commands.Enums;
using DSharpPlus.Entities;

namespace DSharpPlus.CommandAll.Commands.Converters
{
    /// <inheritdoc cref="IArgumentConverter{T}"/>
    public sealed record Int32ArgumentConverter : IArgumentConverter<int>
    {
        /// <inheritdoc/>
        public ApplicationCommandOptionType OptionType { get; init; } = ApplicationCommandOptionType.Integer;

        /// <inheritdoc/>
        public ArgumentParsingBehavior ParsingBehavior { get; init; } = ArgumentParsingBehavior.Static;

        /// <inheritdoc/>
        public Task<Optional<int>> ConvertAsync(CommandContext context, string value, CommandParameter? parameter = null) => Task.FromResult(int.TryParse(value, out int result)
            ? Optional.FromValue(result)
            : Optional.FromNoValue<int>());

        /// <inheritdoc/>
        public bool CanConvert(Type type) => type == typeof(int);
    }
}
