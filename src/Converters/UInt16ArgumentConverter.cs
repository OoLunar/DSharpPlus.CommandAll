using System.Threading.Tasks;
using DSharpPlus;
using DSharpPlus.Entities;
using OoLunar.DSharpPlus.CommandAll.Commands;
using OoLunar.DSharpPlus.CommandAll.Commands.Arguments;

namespace OoLunar.DSharpPlus.CommandAll.Converters
{
    public sealed class UInt16ArgumentConverter : IArgumentConverter<ushort>
    {
        public ApplicationCommandOptionType Type { get; init; } = ApplicationCommandOptionType.Integer;

        public Task<Optional<ushort>> ConvertAsync(CommandContext context, CommandParameter parameter, string value) => Task.FromResult(ushort.TryParse(value, out ushort result) ? Optional.FromValue(result) : Optional.FromNoValue<ushort>());
    }
}
