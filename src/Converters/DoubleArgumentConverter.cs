using System.Threading.Tasks;
using DSharpPlus.CommandAll.Commands;
using DSharpPlus.Entities;

namespace DSharpPlus.CommandAll.Converters
{
    public sealed class DoubleArgumentConverter : IArgumentConverter<double>
    {
        public static ApplicationCommandOptionType OptionType { get; } = ApplicationCommandOptionType.Number;

        public Task<Optional<double>> ConvertAsync(CommandContext context, CommandParameter parameter, string value) => Task.FromResult(double.TryParse(value, out double result) ? Optional.FromValue(result) : Optional.FromNoValue<double>());
    }
}
