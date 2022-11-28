using System.Threading.Tasks;
using DSharpPlus;
using DSharpPlus.Entities;
using OoLunar.DSharpPlus.CommandAll.Commands;
using OoLunar.DSharpPlus.CommandAll.Commands.Arguments;
using OoLunar.DSharpPlus.CommandAll.Commands.System.Commands;

namespace OoLunar.DSharpPlus.CommandAll.Converters
{
    public sealed class FloatArgumentConverter : IArgumentConverter<float>
    {
        public static ApplicationCommandOptionType OptionType { get; } = ApplicationCommandOptionType.Number;

        public Task<Optional<float>> ConvertAsync(CommandContext context, CommandParameter parameter, string value) => Task.FromResult(float.TryParse(value, out float result) ? Optional.FromValue(result) : Optional.FromNoValue<float>());
    }
}
