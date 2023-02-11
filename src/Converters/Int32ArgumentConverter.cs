using System.Threading.Tasks;
using DSharpPlus.CommandAll.Commands;
using DSharpPlus.Entities;

namespace DSharpPlus.CommandAll.Converters
{
    public sealed class Int32ArgumentConverter : IArgumentConverter<int>
    {
        public ApplicationCommandOptionType OptionType { get; init; } = ApplicationCommandOptionType.Integer;

        public Task<Optional<int>> ConvertAsync(CommandContext context, string value, CommandParameter? parameter = null) => Task.FromResult(int.TryParse(value, out int result) ? Optional.FromValue(result) : Optional.FromNoValue<int>());
    }
}
