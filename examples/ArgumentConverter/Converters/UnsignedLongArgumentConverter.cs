using System.Globalization;
using System.Threading.Tasks;
using DSharpPlus;
using DSharpPlus.Entities;
using OoLunar.DSharpPlus.CommandAll.Commands;
using OoLunar.DSharpPlus.CommandAll.Commands.Arguments;

namespace OoLunar.DSharpPlus.CommandAll.Examples.ArgumentConverter.Converters
{
    public sealed class UnsignedLongArgumentConverter : IArgumentConverter<ulong>
    {
        public static readonly ApplicationCommandOptionType ArgumentType = ApplicationCommandOptionType.Integer;

        public Task<Optional<ulong>> ConvertAsync(CommandContext context, CommandParameter parameter, string value) => ulong.TryParse(value, NumberStyles.None, CultureInfo.InvariantCulture, out ulong result) ? Task.FromResult(Optional.FromValue(result)) : Task.FromResult(Optional.FromNoValue<ulong>());
    }
}
