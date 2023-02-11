using System.Threading.Tasks;
using DSharpPlus.CommandAll.Commands;
using DSharpPlus.Entities;

namespace DSharpPlus.CommandAll.Converters
{
    public sealed class BooleanArgumentConverter : IArgumentConverter<bool>
    {
        public ApplicationCommandOptionType OptionType { get; init; } = ApplicationCommandOptionType.Boolean;

        public Task<Optional<bool>> ConvertAsync(CommandContext context, string value, CommandParameter? parameter = null) => value.Trim().ToLowerInvariant() switch
        {
            "true" or "yes" or "on" or "y" or "1" => Task.FromResult(Optional.FromValue(true)),
            "false" or "no" or "off" or "n" or "0" => Task.FromResult(Optional.FromValue(false)),
            _ => Task.FromResult(Optional.FromNoValue<bool>()),
        };
    }
}
