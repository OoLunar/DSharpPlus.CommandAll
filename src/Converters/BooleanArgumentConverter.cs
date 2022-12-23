using System.Threading.Tasks;
using DSharpPlus.CommandAll.Commands;
using DSharpPlus.CommandAll.Commands.Arguments;
using DSharpPlus.Entities;

namespace DSharpPlus.CommandAll.Converters
{
    public sealed class BooleanArgumentConverter : IArgumentConverter<bool>
    {
        public static ApplicationCommandOptionType OptionType { get; } = ApplicationCommandOptionType.Boolean;

        public Task<Optional<bool>> ConvertAsync(CommandContext context, CommandParameter parameter, string value) => value.Trim().ToLowerInvariant() switch
        {
            "true" or "yes" or "on" or "y" or "1" => Task.FromResult(Optional.FromValue(true)),
            "false" or "no" or "off" or "n" or "0" => Task.FromResult(Optional.FromValue(false)),
            _ => Task.FromResult(Optional.FromNoValue<bool>()),
        };
    }
}
