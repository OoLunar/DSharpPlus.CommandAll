using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using DSharpPlus;
using DSharpPlus.Entities;
using OoLunar.DSharpPlus.CommandAll.Commands;
using OoLunar.DSharpPlus.CommandAll.Commands.Arguments;
using OoLunar.DSharpPlus.CommandAll.Commands.System.Commands;

namespace OoLunar.DSharpPlus.CommandAll.Converters
{
    public sealed partial class DiscordRoleArgumentConverter : IArgumentConverter<DiscordRole>
    {
        public static ApplicationCommandOptionType OptionType { get; } = ApplicationCommandOptionType.Role;
        private static readonly Regex RoleRegex = RoleRegexMethod();

        [SuppressMessage("Roslyn", "IDE0046", Justification = "Silence the ternary rabbit hole.")]
        public Task<Optional<DiscordRole>> ConvertAsync(CommandContext context, CommandParameter parameter, string value)
        {
            // value can be a raw channel id or a channel mention. The regex will match both.
            Match match = RoleRegex.Match(value);
            if (!match.Success || !ulong.TryParse(match.Captures[0].ValueSpan, NumberStyles.Number, CultureInfo.InvariantCulture, out ulong roleId))
            {
                return Task.FromResult(Optional.FromNoValue<DiscordRole>());
            }

            if (context.IsSlashCommand && context.Interaction!.Data.Resolved.Roles is not null && context.Interaction.Data.Resolved.Roles.TryGetValue(roleId, out DiscordRole? role))
            {
                return Task.FromResult(Optional.FromValue(role));
            }

            role = context.Guild!.GetRole(roleId);
            return Task.FromResult(role is not null ? Optional.FromValue(role) : Optional.FromNoValue<DiscordRole>());
        }

        [GeneratedRegex(@"(\d+)|^<@&(\d+?)>$", RegexOptions.Compiled | RegexOptions.ECMAScript)]
        private static partial Regex RoleRegexMethod();
    }
}
