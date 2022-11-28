using System.Threading.Tasks;
using DSharpPlus;
using DSharpPlus.Entities;
using OoLunar.DSharpPlus.CommandAll.Commands;
using OoLunar.DSharpPlus.CommandAll.Commands.Arguments;
using OoLunar.DSharpPlus.CommandAll.Commands.System.Commands;

namespace OoLunar.DSharpPlus.CommandAll.Converters
{
    public sealed class ByteArgumentConverter : IArgumentConverter<byte>
    {
        public static ApplicationCommandOptionType OptionType { get; } = ApplicationCommandOptionType.Integer;

        public Task<Optional<byte>> ConvertAsync(CommandContext context, CommandParameter parameter, string value) => Task.FromResult(byte.TryParse(value, out byte result) ? Optional.FromValue(result) : Optional.FromNoValue<byte>());
    }
}
