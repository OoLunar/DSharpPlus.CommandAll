using System;
using System.Threading.Tasks;
using DSharpPlus;
using DSharpPlus.Entities;
using OoLunar.DSharpPlus.CommandAll.Commands;
using OoLunar.DSharpPlus.CommandAll.Commands.Arguments;

namespace OoLunar.DSharpPlus.CommandAll.Examples.ArgumentConverters.ArgumentConverters
{
    public sealed class UlidArgumentConverter : IArgumentConverter<Ulid>
    {
        public ApplicationCommandOptionType Type { get; init; } = ApplicationCommandOptionType.String;

        public Task<Optional<Ulid>> ConvertAsync(CommandContext context, CommandParameter parameter, string value) => Ulid.TryParse(value, out Ulid ulid)
            ? Task.FromResult(Optional.FromValue(ulid))
            : Task.FromResult(Optional.FromNoValue<Ulid>());
    }
}
