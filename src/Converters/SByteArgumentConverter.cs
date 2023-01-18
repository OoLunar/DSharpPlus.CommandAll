using System.Threading.Tasks;
using DSharpPlus.CommandAll.Commands;
using DSharpPlus.Entities;

namespace DSharpPlus.CommandAll.Converters
{
    public sealed class SByteArgumentConverter : IArgumentConverter<sbyte>
    {
        public static ApplicationCommandOptionType OptionType { get; } = ApplicationCommandOptionType.Integer;

        public Task<Optional<sbyte>> ConvertAsync(CommandContext context, CommandParameter parameter, string value) => Task.FromResult(sbyte.TryParse(value, out sbyte result) ? Optional.FromValue(result) : Optional.FromNoValue<sbyte>());
    }
}
