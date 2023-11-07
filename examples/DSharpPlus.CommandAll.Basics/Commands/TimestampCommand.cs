using System;
using System.Threading.Tasks;
using DSharpPlus.CommandAll.Commands;
using DSharpPlus.CommandAll.Commands.Attributes;

namespace DSharpPlus.CommandAll.Examples.Basics.Commands
{
    public sealed class TimestampCommand
    {
        [Command("timestamp")]
        public static async Task ExecuteAsync(CommandContext context, TimestampFormat format = TimestampFormat.LongDateTime) => await context.RespondAsync($"Timestamp: {Formatter.Timestamp(DateTime.UtcNow, format)}");
    }
}
