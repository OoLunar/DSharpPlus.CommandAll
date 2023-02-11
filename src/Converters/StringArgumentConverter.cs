using System.Threading.Tasks;
using DSharpPlus.CommandAll.Commands;
using DSharpPlus.Entities;

namespace DSharpPlus.CommandAll.Converters
{
    public sealed class StringArgumentConverter : IArgumentConverter<string>
    {
        public ApplicationCommandOptionType OptionType { get; init; } = ApplicationCommandOptionType.String;

        public Task<Optional<string>> ConvertAsync(CommandContext context, string value, CommandParameter? parameter = null) => Task.FromResult(Optional.FromValue(value.ToString()));
    }
}
