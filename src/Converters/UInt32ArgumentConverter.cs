using System.Threading.Tasks;
using DSharpPlus;
using DSharpPlus.Entities;
using OoLunar.DSharpPlus.CommandAll.Commands;
using OoLunar.DSharpPlus.CommandAll.Commands.Arguments;

namespace OoLunar.DSharpPlus.CommandAll.Converters
{
    public sealed class UInt32ArgumentConverter : IArgumentConverter<uint>
    {
        public static ApplicationCommandOptionType OptionType { get; } = ApplicationCommandOptionType.Integer;

        public Task<Optional<uint>> ConvertAsync(CommandContext context, CommandParameter parameter, string value) => Task.FromResult(uint.TryParse(value, out uint result) ? Optional.FromValue(result) : Optional.FromNoValue<uint>());
    }
}
