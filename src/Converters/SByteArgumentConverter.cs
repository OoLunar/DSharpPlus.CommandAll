using System.Threading.Tasks;
using DSharpPlus.CommandAll.Commands;
using DSharpPlus.Entities;

namespace DSharpPlus.CommandAll.Converters
{
    public sealed class SByteArgumentConverter : IArgumentConverter<sbyte>
    {
        public ApplicationCommandOptionType OptionType { get; init; } = ApplicationCommandOptionType.Integer;

        public Task<Optional<sbyte>> ConvertAsync(CommandContext context, string value, CommandParameter? parameter = null) => Task.FromResult(sbyte.TryParse(value, out sbyte result) ? Optional.FromValue(result) : Optional.FromNoValue<sbyte>());
    }
}
