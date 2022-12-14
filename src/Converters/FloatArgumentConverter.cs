using System.Threading.Tasks;
using DSharpPlus.CommandAll.Commands;
using DSharpPlus.CommandAll.Commands.Arguments;
using DSharpPlus.Entities;

namespace DSharpPlus.CommandAll.Converters
{
    public sealed class FloatArgumentConverter : IArgumentConverter<float>
    {
        public static ApplicationCommandOptionType OptionType { get; } = ApplicationCommandOptionType.Number;

        public Task<Optional<float>> ConvertAsync(CommandContext context, CommandParameter parameter, string value) => Task.FromResult(float.TryParse(value, out float result) ? Optional.FromValue(result) : Optional.FromNoValue<float>());
    }
}
