using System.Threading.Tasks;
using DSharpPlus.CommandAll.Commands;
using DSharpPlus.Entities;

namespace DSharpPlus.CommandAll.Converters
{
    public sealed class UInt16ArgumentConverter : IArgumentConverter<ushort>
    {
        public ApplicationCommandOptionType OptionType { get; init; } = ApplicationCommandOptionType.Integer;

        public Task<Optional<ushort>> ConvertAsync(CommandContext context, string value, CommandParameter? parameter = null) => Task.FromResult(ushort.TryParse(value, out ushort result) ? Optional.FromValue(result) : Optional.FromNoValue<ushort>());
    }
}
