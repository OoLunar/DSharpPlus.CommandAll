using System.Globalization;
using System.Text;
using System.Threading.Tasks;
using DSharpPlus.CommandAll.Commands;
using DSharpPlus.CommandAll.Commands.Attributes;
using DSharpPlus.Entities;

namespace DSharpPlus.CommandAll.Examples.Basics.Commands
{
    public sealed class TimeOfCommand
    {
        [Command("time_of")]
        public static async Task ExecuteAsync(CommandContext context, DiscordMessage message, DiscordMessage? other_message = null)
        {
            if (other_message is null)
            {
                await context.RespondAsync($"{Formatter.InlineCode(message.Id.ToString(CultureInfo.InvariantCulture))} => {Formatter.InlineCode(message.Id.GetSnowflakeTime().ToString("yyyy'-'MM'-'dd' 'HH':'mm':'ss'.'ffff", CultureInfo.InvariantCulture))}");
                return;
            }

            StringBuilder stringBuilder = new();
            stringBuilder.Append("```md\n");
            stringBuilder.Append($"{message.Id.ToString(CultureInfo.InvariantCulture)} => {message.Id.GetSnowflakeTime().ToString("yyyy'-'MM'-'dd' 'HH':'mm':'ss'.'ffff", CultureInfo.InvariantCulture)}\n");
            stringBuilder.Append($"{other_message.Id.ToString(CultureInfo.InvariantCulture)} => {other_message.Id.GetSnowflakeTime().ToString("yyyy'-'MM'-'dd' 'HH':'mm':'ss'.'ffff", CultureInfo.InvariantCulture)}\n");
            stringBuilder.Append("Difference: ");
            stringBuilder.Append((message.Id.GetSnowflakeTime() - other_message.Id.GetSnowflakeTime()).ToString("g", CultureInfo.InvariantCulture));
            stringBuilder.Append("```");
            await context.RespondAsync(stringBuilder.ToString());
        }
    }
}
