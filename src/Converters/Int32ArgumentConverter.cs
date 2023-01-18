using System.Threading.Tasks;
using DSharpPlus.CommandAll.Commands;
using DSharpPlus.Entities;

namespace DSharpPlus.CommandAll.Converters
{
    public sealed class Int32ArgumentConverter : IArgumentConverter<int>
    {
        public static ApplicationCommandOptionType OptionType { get; } = ApplicationCommandOptionType.Integer;

        public Task<Optional<int>> ConvertAsync(CommandContext context, CommandParameter parameter, string value) => Task.FromResult(int.TryParse(value, out int result) ? Optional.FromValue(result) : Optional.FromNoValue<int>());
    }
}
