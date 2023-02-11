using System.Threading.Tasks;
using DSharpPlus.CommandAll.Commands;
using DSharpPlus.Entities;

namespace DSharpPlus.CommandAll.Converters
{
    public sealed class Int64ArgumentConverter : IArgumentConverter<long>
    {
        public ApplicationCommandOptionType OptionType { get; init; } = ApplicationCommandOptionType.String;

        public Task<Optional<long>> ConvertAsync(CommandContext context, string value, CommandParameter? parameter = null) => Task.FromResult(long.TryParse(value, out long result) ? Optional.FromValue(result) : Optional.FromNoValue<long>());
    }
}
