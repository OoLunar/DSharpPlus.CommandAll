using System;
using System.Threading.Tasks;
using DSharpPlus.CommandAll.Commands;
using DSharpPlus.Entities;

namespace DSharpPlus.CommandAll.Converters
{
    public sealed class DateTimeArgumentConverter : IArgumentConverter<DateTime>
    {
        public ApplicationCommandOptionType OptionType { get; init; } = ApplicationCommandOptionType.String;

        public Task<Optional<DateTime>> ConvertAsync(CommandContext context, string value, CommandParameter? parameter = null) => DateTime.TryParse(value, out DateTime result)
            ? Task.FromResult(Optional.FromValue(result.ToUniversalTime()))
            : Task.FromResult(Optional.FromNoValue<DateTime>());
    }
}
