using System;
using System.Threading.Tasks;
using DSharpPlus.CommandAll.Commands;
using DSharpPlus.Entities;

namespace DSharpPlus.CommandAll.Converters
{
    public sealed class EnumArgumentConverter : IArgumentConverter<Enum>
    {
        public static ApplicationCommandOptionType OptionType { get; } = ApplicationCommandOptionType.Integer;

        public Task<Optional<Enum>> ConvertAsync(CommandContext context, CommandParameter parameter, string value) => Task.FromResult(Enum.TryParse(parameter.ParameterInfo.ParameterType, value, true, out object? result) ? Optional.FromValue((Enum)result) : Optional.FromNoValue<Enum>());
    }
}
