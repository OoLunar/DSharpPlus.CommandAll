using System.Threading.Tasks;
using DSharpPlus;
using DSharpPlus.Entities;
using OoLunar.DSharpPlus.CommandAll.Commands;
using OoLunar.DSharpPlus.CommandAll.Commands.Arguments;

namespace OoLunar.DSharpPlus.CommandAll.Converters
{
    public sealed class DoubleArgumentConverter : IArgumentConverter<double>
    {
        public static ApplicationCommandOptionType OptionType { get; } = ApplicationCommandOptionType.Number;

        public Task<Optional<double>> ConvertAsync(CommandContext context, CommandParameter parameter, string value) => Task.FromResult(double.TryParse(value, out double result) ? Optional.FromValue(result) : Optional.FromNoValue<double>());
    }
}
