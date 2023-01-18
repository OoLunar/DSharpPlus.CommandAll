using System.Threading.Tasks;
using DSharpPlus.CommandAll.Commands;
using DSharpPlus.Entities;

namespace DSharpPlus.CommandAll.Converters
{
    public sealed class ByteArgumentConverter : IArgumentConverter<byte>
    {
        public static ApplicationCommandOptionType OptionType { get; } = ApplicationCommandOptionType.Integer;

        public Task<Optional<byte>> ConvertAsync(CommandContext context, CommandParameter parameter, string value) => Task.FromResult(byte.TryParse(value, out byte result) ? Optional.FromValue(result) : Optional.FromNoValue<byte>());
    }
}
