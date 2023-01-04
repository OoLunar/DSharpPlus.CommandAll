using System;
using System.Threading.Tasks;
using DSharpPlus.CommandAll.Commands;
using DSharpPlus.CommandAll.Commands.Converters;
using DSharpPlus.CommandAll.Commands.Enums;
using DSharpPlus.Entities;

namespace DSharpPlus.CommandAll.Examples.ArgumentConverters.ArgumentConverters
{
    public sealed class UlidArgumentConverter : ArgumentConverter<Ulid>
    {
        public override ApplicationCommandOptionType OptionType { get; } = ApplicationCommandOptionType.String;
        public override ArgumentParsingBehavior ParsingBehavior { get; } = ArgumentParsingBehavior.Static;

        public override Task<Optional<Ulid>> ConvertAsync(CommandContext context, string value, CommandParameter? parameter = null) => Ulid.TryParse(value, out Ulid ulid)
            ? Task.FromResult(Optional.FromValue(ulid))
            : Task.FromResult(Optional.FromNoValue<Ulid>());

        public override bool CanConvert(Type type) => type == typeof(Ulid);
    }
}
