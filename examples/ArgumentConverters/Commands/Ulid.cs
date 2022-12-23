using System;
using System.ComponentModel;
using System.Threading.Tasks;
using DSharpPlus.CommandAll.Attributes;
using DSharpPlus.CommandAll.Commands;

namespace DSharpPlus.CommandAll.Examples.ArgumentConverters.Commands
{
    [Command("ulid"), Description("Commands for manipulating Ulids.")]
    public sealed class UlidCommands : BaseCommand
    {
        [Command("generate"), Description("Generates a new Ulid.")]
        public static Task GenerateAsync(CommandContext context) => context.ReplyAsync(Formatter.InlineCode(Ulid.NewUlid().ToString()));

        [Command("parse"), Description("Parses a Ulid from a string.")]
        public static Task ParseAsync(CommandContext context, [Description("The Ulid to parse.")] Ulid ulid) => context.ReplyAsync($"Ulid {Formatter.InlineCode(ulid.ToString())} was created at {Formatter.Timestamp(ulid.Time, TimestampFormat.LongDateTime)}.");
    }
}
