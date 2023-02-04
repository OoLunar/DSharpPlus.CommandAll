using System;
using System.Threading.Tasks;
using DSharpPlus.CommandAll.Commands;
using DSharpPlus.Entities;

namespace DSharpPlus.CommandAll.Converters
{
    public sealed class DateTimeOffsetArgumentConverter : IArgumentConverter<DateTimeOffset>
    {
        public ApplicationCommandOptionType OptionType { get; init; } = ApplicationCommandOptionType.String;

        public Task<Optional<DateTimeOffset>> ConvertAsync(CommandContext context, CommandParameter parameter, string value) => DateTimeOffset.TryParse(value, out DateTimeOffset result)
            ? Task.FromResult(Optional.FromValue(result.ToUniversalTime()))
            : Task.FromResult(Optional.FromNoValue<DateTimeOffset>());
    }
}
