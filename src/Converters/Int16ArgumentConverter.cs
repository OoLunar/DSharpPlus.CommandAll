using System.Threading.Tasks;
using DSharpPlus.CommandAll.Commands;
using DSharpPlus.Entities;

namespace DSharpPlus.CommandAll.Converters
{
    public sealed class Int16ArgumentConverter : IArgumentConverter<short>
    {
        public ApplicationCommandOptionType OptionType { get; init; } = ApplicationCommandOptionType.Integer;

        public Task<Optional<short>> ConvertAsync(CommandContext context, string value, CommandParameter? parameter = null) => Task.FromResult(short.TryParse(value, out short result) ? Optional.FromValue(result) : Optional.FromNoValue<short>());
    }
}
