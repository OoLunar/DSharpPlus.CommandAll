using System;
using System.Threading.Tasks;
using DSharpPlus;
using DSharpPlus.Entities;
using OoLunar.DSharpPlus.CommandAll.Commands;
using OoLunar.DSharpPlus.CommandAll.Commands.Arguments;
using OoLunar.DSharpPlus.CommandAll.Commands.System.Commands;

namespace OoLunar.DSharpPlus.CommandAll.Converters
{
    public sealed class DateTimeArgumentConverter : IArgumentConverter<DateTime>
    {
        public static ApplicationCommandOptionType OptionType => ApplicationCommandOptionType.String;

        public Task<Optional<DateTime>> ConvertAsync(CommandContext context, CommandParameter parameter, string value) => DateTime.TryParse(value, out DateTime result)
            ? Task.FromResult(Optional.FromValue(result.ToUniversalTime()))
            : Task.FromResult(Optional.FromNoValue<DateTime>());
    }
}
