using System;
using System.Threading.Tasks;
using DSharpPlus;
using DSharpPlus.Entities;
using OoLunar.DSharpPlus.CommandAll.Commands;
using OoLunar.DSharpPlus.CommandAll.Commands.Arguments;

namespace OoLunar.DSharpPlus.CommandAll.Converters
{
    public sealed class EnumArgumentConverter : IArgumentConverter<Enum>
    {
        public ApplicationCommandOptionType Type { get; init; } = ApplicationCommandOptionType.Integer;

        public Task<Optional<Enum>> ConvertAsync(CommandContext context, CommandParameter parameter, string value) => Task.FromResult(Enum.TryParse(parameter.Type, value, true, out object? result) ? Optional.FromValue((Enum)result) : Optional.FromNoValue<Enum>());
    }
}
