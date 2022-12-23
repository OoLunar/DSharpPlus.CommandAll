using System.Threading.Tasks;
using DSharpPlus.CommandAll.Commands;
using DSharpPlus.CommandAll.Commands.Arguments;
using DSharpPlus.Entities;

namespace DSharpPlus.CommandAll.Converters
{
    public sealed class UInt32ArgumentConverter : IArgumentConverter<uint>
    {
        public static ApplicationCommandOptionType OptionType { get; } = ApplicationCommandOptionType.Integer;

        public Task<Optional<uint>> ConvertAsync(CommandContext context, CommandParameter parameter, string value) => Task.FromResult(uint.TryParse(value, out uint result) ? Optional.FromValue(result) : Optional.FromNoValue<uint>());
    }
}
