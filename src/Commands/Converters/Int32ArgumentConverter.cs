using System.Threading.Tasks;
using DSharpPlus.CommandAll.Commands.Enums;
using DSharpPlus.Entities;

namespace DSharpPlus.CommandAll.Commands.Converters
{
    /// <inheritdoc cref="ArgumentConverter{T}"/>
    public sealed class Int32ArgumentConverter : ArgumentConverter<int>
    {
        /// <inheritdoc/>
        public override ApplicationCommandOptionType OptionType { get; } = ApplicationCommandOptionType.Integer;

        /// <inheritdoc/>
        public override ArgumentParsingBehavior ParsingBehavior { get; } = ArgumentParsingBehavior.Static;

        /// <inheritdoc/>
        public override Task<Optional<int>> ConvertAsync(CommandContext context, string value, CommandParameter? parameter = null) => Task.FromResult(int.TryParse(value, out int result)
            ? Optional.FromValue(result)
            : Optional.FromNoValue<int>());
    }
}
