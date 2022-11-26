using System.Threading.Tasks;
using DSharpPlus;
using DSharpPlus.Entities;
using OoLunar.DSharpPlus.CommandAll.Commands;
using OoLunar.DSharpPlus.CommandAll.Commands.Arguments;

namespace OoLunar.DSharpPlus.CommandAll.Converters
{
    public sealed class Int16ArgumentConverter : IArgumentConverter<short>
    {
        public static ApplicationCommandOptionType OptionType { get; } = ApplicationCommandOptionType.Integer;

        public Task<Optional<short>> ConvertAsync(CommandContext context, CommandParameter parameter, string value) => Task.FromResult(short.TryParse(value, out short result) ? Optional.FromValue(result) : Optional.FromNoValue<short>());
    }
}
