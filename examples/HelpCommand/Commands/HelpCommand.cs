using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using DSharpPlus;
using DSharpPlus.Entities;
using OoLunar.DSharpPlus.CommandAll.Attributes;
using OoLunar.DSharpPlus.CommandAll.Commands;
using OoLunar.DSharpPlus.CommandAll.Commands.System.Commands;

namespace OoLunar.DSharpPlus.CommandAll.Examples.HelpCommand.Commands
{
    public sealed class HelpCommand : BaseCommand
    {
        [Command("help"), Description("Lists the commands available or provides more information on the requested command.")]
        public static Task EchoAsync(CommandContext context, [Description("The command to search for.")] string? command = null)
        {
            DiscordMessageBuilder messageBuilder = new();
            if (command is null)
            {
                DiscordEmbedBuilder embedBuilder = new() { Color = new DiscordColor("#6b73db") };
                foreach (Command distinctCommand in context.Extension.CommandManager.Commands.Values.Distinct())
                {
                    embedBuilder.AddField(distinctCommand.Name, distinctCommand.Description);
                }
                messageBuilder.AddEmbed(embedBuilder);
            }
            else
            {
                if (context.Extension.CommandManager.Commands.TryGetValue(command.ToLowerInvariant(), out Command? foundCommand))
                {
                    DiscordEmbedBuilder embedBuilder = new()
                    {
                        Color = new DiscordColor("#6b73db"),
                        Title = foundCommand.Name,
                        Description = foundCommand.Description
                    };

                    foreach (CommandParameter parameter in foundCommand.Overloads[0].Parameters)
                    {
                        embedBuilder.AddField($"Parameter '{parameter.Name}'", parameter.Description);
                    }
                    messageBuilder.AddEmbed(embedBuilder);
                }
                else
                {
                    messageBuilder.WithContent($":x: Command {Formatter.InlineCode(Formatter.Sanitize(command))} not found.");
                }
            }

            return context.ReplyAsync(messageBuilder);
        }
    }
}
