using System.Threading.Tasks;
using DSharpPlus;
using DSharpPlus.Entities;
using OoLunar.DSharpPlus.CommandAll.Commands;
using OoLunar.DSharpPlus.CommandAll.Commands.Arguments;

namespace OoLunar.DSharpPlus.CommandAll.Converters
{
    public sealed class UInt64ArgumentConverter : IArgumentConverter<ulong>
    {
        public ApplicationCommandOptionType Type { get; init; } = ApplicationCommandOptionType.Integer;

        public Task<Optional<ulong>> ConvertAsync(CommandContext context, CommandParameter parameter, string value) => Task.FromResult(ulong.TryParse(value, out ulong result) ? Optional.FromValue(result) : Optional.FromNoValue<ulong>());
    }
}
