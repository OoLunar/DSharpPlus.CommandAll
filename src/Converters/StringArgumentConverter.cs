using System.Threading.Tasks;
using DSharpPlus.CommandAll.Commands;
using DSharpPlus.CommandAll.Commands.Arguments;
using DSharpPlus.Entities;

namespace DSharpPlus.CommandAll.Converters
{
    public sealed class StringArgumentConverter : IArgumentConverter<string>
    {
        public static ApplicationCommandOptionType OptionType { get; } = ApplicationCommandOptionType.String;

        public Task<Optional<string>> ConvertAsync(CommandContext context, CommandParameter parameter, string value) => Task.FromResult(Optional.FromValue(value.ToString()));
    }
}
