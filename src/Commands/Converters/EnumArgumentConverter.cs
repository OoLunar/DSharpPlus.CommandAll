using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using DSharpPlus.CommandAll.Commands.Enums;
using DSharpPlus.Entities;

namespace DSharpPlus.CommandAll.Commands.Converters
{
    /// <inheritdoc cref="IArgumentConverter{T}"/>
    public sealed class EnumArgumentConverter : IArgumentConverter<Enum>
    {
        /// <inheritdoc/>
        public ApplicationCommandOptionType OptionType => ApplicationCommandOptionType.Integer;

        /// <inheritdoc/>
        public ArgumentParsingBehavior ParsingBehavior => ArgumentParsingBehavior.RequiresCommandParameter;

        /// <inheritdoc/>
        public bool CanConvert(Type type) => type == typeof(Enum);

        /// <inheritdoc/>
        [SuppressMessage("Style", "IDE0046:Convert to conditional expression", Justification = "This is more readable.")]
        public Task<Optional<Enum>> ConvertAsync(CommandContext context, string value, CommandParameter? parameter = null)
        {
            if (parameter is null)
            {
                throw new ArgumentNullException(nameof(parameter));
            }
            else if (Enum.TryParse(parameter.ParameterInfo.ParameterType, value, true, out object? result))
            {
                return Task.FromResult(Optional.FromValue((Enum)result));
            }
            else
            {
                return Task.FromResult(Optional.FromNoValue<Enum>());
            }
        }
    }
}
