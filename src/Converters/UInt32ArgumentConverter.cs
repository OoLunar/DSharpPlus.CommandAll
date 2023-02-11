using System.Threading.Tasks;
using DSharpPlus.CommandAll.Commands;
using DSharpPlus.Entities;

namespace DSharpPlus.CommandAll.Converters
{
    public sealed class UInt32ArgumentConverter : IArgumentConverter<uint>
    {
        public ApplicationCommandOptionType OptionType { get; init; } = ApplicationCommandOptionType.Integer;

        public Task<Optional<uint>> ConvertAsync(CommandContext context, string value, CommandParameter? parameter = null) => Task.FromResult(uint.TryParse(value, out uint result) ? Optional.FromValue(result) : Optional.FromNoValue<uint>());
    }
}
