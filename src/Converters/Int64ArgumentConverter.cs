using System.Threading.Tasks;
using DSharpPlus.CommandAll.Commands;
using DSharpPlus.Entities;

namespace DSharpPlus.CommandAll.Converters
{
    public sealed class Int64ArgumentConverter : IArgumentConverter<long>
    {
        public ApplicationCommandOptionType OptionType { get; init; } = ApplicationCommandOptionType.Integer;

        public Task<Optional<long>> ConvertAsync(CommandContext context, CommandParameter parameter, string value) => Task.FromResult(long.TryParse(value, out long result) ? Optional.FromValue(result) : Optional.FromNoValue<long>());
    }
}
