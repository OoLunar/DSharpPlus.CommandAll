using System;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using DSharpPlus.CommandAll.Commands;
using DSharpPlus.CommandAll.Commands.Arguments;
using DSharpPlus.Entities;

namespace DSharpPlus.CommandAll.Converters
{
    public sealed partial class DiscordRoleArgumentConverter : IArgumentConverter<DiscordRole>
    {
        public static ApplicationCommandOptionType OptionType { get; } = ApplicationCommandOptionType.Role;

        [SuppressMessage("Roslyn", "IDE0046", Justification = "Silence the ternary rabbit hole.")]
        public Task<Optional<DiscordRole>> ConvertAsync(CommandContext context, CommandParameter parameter, string value)
        {
            if (!ulong.TryParse(value, NumberStyles.Number, CultureInfo.InvariantCulture, out ulong roleId))
            {
                // value can be a raw channel id or a channel mention. The regex will match both.
                Match match = GetRoleRegex().Match(value);
                if (!match.Success || !ulong.TryParse(match.Captures[0].ValueSpan, NumberStyles.Number, CultureInfo.InvariantCulture, out roleId))
                {
                    // Attempt to find a role by name, case insensitive.
                    DiscordRole? namedRole = context.Guild!.Roles.Values.FirstOrDefault(role => role.Name.Equals(value, StringComparison.OrdinalIgnoreCase));
                    return Task.FromResult(namedRole is not null ? Optional.FromValue(namedRole) : Optional.FromNoValue<DiscordRole>());
                }
            }

            if (context.IsSlashCommand && context.Interaction!.Data.Resolved?.Roles is not null && context.Interaction.Data.Resolved.Roles.TryGetValue(roleId, out DiscordRole? role))
            {
                return Task.FromResult(Optional.FromValue(role));
            }
            else if (context.Guild!.GetRole(roleId) is DiscordRole guildRole)
            {
                return Task.FromResult(Optional.FromValue(guildRole));
            }
            else
            {
                return Task.FromResult(Optional.FromNoValue<DiscordRole>());
            }
        }

        [GeneratedRegex(@"^<@&(\d+?)>$", RegexOptions.Compiled | RegexOptions.ECMAScript)]
        private static partial Regex GetRoleRegex();
    }
}
