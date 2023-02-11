using System.Threading.Tasks;
using DSharpPlus.CommandAll.Commands;
using DSharpPlus.Entities;

namespace DSharpPlus.CommandAll.Converters
{
    public sealed class UInt64ArgumentConverter : IArgumentConverter<ulong>
    {
        public ApplicationCommandOptionType OptionType { get; init; } = ApplicationCommandOptionType.String;

        public Task<Optional<ulong>> ConvertAsync(CommandContext context, string value, CommandParameter? parameter = null) => Task.FromResult(ulong.TryParse(value, out ulong result) ? Optional.FromValue(result) : Optional.FromNoValue<ulong>());
    }
}
