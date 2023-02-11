using System;
using System.Threading.Tasks;
using DSharpPlus.CommandAll.Commands;
using DSharpPlus.Entities;

namespace DSharpPlus.CommandAll.Converters
{
    public sealed class EnumArgumentConverter : IArgumentConverter<Enum>
    {
        public ApplicationCommandOptionType OptionType { get; init; } = ApplicationCommandOptionType.Integer;

        public Task<Optional<Enum>> ConvertAsync(CommandContext context, string value, CommandParameter? parameter = null) => parameter is null
            ? Task.FromResult(Optional.FromNoValue<Enum>())
            : Task.FromResult(Enum.TryParse(parameter.ParameterInfo.ParameterType, value, true, out object? result) ? Optional.FromValue((Enum)result) : Optional.FromNoValue<Enum>());
    }
}
