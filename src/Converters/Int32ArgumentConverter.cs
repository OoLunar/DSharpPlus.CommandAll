using System.Threading.Tasks;
using DSharpPlus;
using DSharpPlus.Entities;
using OoLunar.DSharpPlus.CommandAll.Commands;
using OoLunar.DSharpPlus.CommandAll.Commands.Arguments;

namespace OoLunar.DSharpPlus.CommandAll.Converters
{
    public sealed class Int32ArgumentConverter : IArgumentConverter<int>
    {
        public static ApplicationCommandOptionType OptionType { get; } = ApplicationCommandOptionType.Integer;

        public Task<Optional<int>> ConvertAsync(CommandContext context, CommandParameter parameter, string value) => Task.FromResult(int.TryParse(value, out int result) ? Optional.FromValue(result) : Optional.FromNoValue<int>());
    }
}
