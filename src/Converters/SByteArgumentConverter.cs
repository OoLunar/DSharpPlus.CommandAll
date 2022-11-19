using System.Threading.Tasks;
using DSharpPlus;
using DSharpPlus.Entities;
using OoLunar.DSharpPlus.CommandAll.Commands;
using OoLunar.DSharpPlus.CommandAll.Commands.Arguments;

namespace OoLunar.DSharpPlus.CommandAll.Converters
{
    public sealed class SByteArgumentConverter : IArgumentConverter<sbyte>
    {
        public ApplicationCommandOptionType Type { get; init; } = ApplicationCommandOptionType.Integer;

        public Task<Optional<sbyte>> ConvertAsync(CommandContext context, CommandParameter parameter, string value) => Task.FromResult(sbyte.TryParse(value, out sbyte result) ? Optional.FromValue(result) : Optional.FromNoValue<sbyte>());
    }
}
