using System.Threading.Tasks;
using DSharpPlus.CommandAll.Commands;
using DSharpPlus.Entities;

namespace DSharpPlus.CommandAll.Converters
{
    public sealed class Int16ArgumentConverter : IArgumentConverter<short>
    {
        public static ApplicationCommandOptionType OptionType { get; } = ApplicationCommandOptionType.Integer;

        public Task<Optional<short>> ConvertAsync(CommandContext context, CommandParameter parameter, string value) => Task.FromResult(short.TryParse(value, out short result) ? Optional.FromValue(result) : Optional.FromNoValue<short>());
    }
}
