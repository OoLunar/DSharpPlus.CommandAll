using System.Threading.Tasks;
using DSharpPlus.CommandAll.Commands;
using DSharpPlus.Entities;

namespace DSharpPlus.CommandAll.Converters
{
    public sealed class ByteArgumentConverter : IArgumentConverter<byte>
    {
        public ApplicationCommandOptionType OptionType { get; init; } = ApplicationCommandOptionType.Integer;

        public Task<Optional<byte>> ConvertAsync(CommandContext context, string value, CommandParameter? parameter = null) => Task.FromResult(byte.TryParse(value, out byte result) ? Optional.FromValue(result) : Optional.FromNoValue<byte>());
    }
}
