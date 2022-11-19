using System.Threading.Tasks;
using DSharpPlus;
using DSharpPlus.Entities;
using OoLunar.DSharpPlus.CommandAll.Commands;
using OoLunar.DSharpPlus.CommandAll.Commands.Arguments;

namespace OoLunar.DSharpPlus.CommandAll.Converters
{
    public sealed class StringArgumentConverter : IArgumentConverter<string>
    {
        public ApplicationCommandOptionType Type { get; init; } = ApplicationCommandOptionType.String;

        public Task<Optional<string>> ConvertAsync(CommandContext context, CommandParameter parameter, string value) => Task.FromResult(Optional.FromValue(value.ToString()));
    }
}
