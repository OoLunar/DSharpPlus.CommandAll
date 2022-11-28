using System.Threading.Tasks;
using DSharpPlus;
using DSharpPlus.Entities;
using OoLunar.DSharpPlus.CommandAll.Commands;
using OoLunar.DSharpPlus.CommandAll.Commands.Arguments;
using OoLunar.DSharpPlus.CommandAll.Commands.System.Commands;

namespace OoLunar.DSharpPlus.CommandAll.Converters
{
    public sealed class StringArgumentConverter : IArgumentConverter<string>
    {
        public static ApplicationCommandOptionType OptionType { get; } = ApplicationCommandOptionType.String;

        public Task<Optional<string>> ConvertAsync(CommandContext context, CommandParameter parameter, string value) => Task.FromResult(Optional.FromValue(value.ToString()));
    }
}
