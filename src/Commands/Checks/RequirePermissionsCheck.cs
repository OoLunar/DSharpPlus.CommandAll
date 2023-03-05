using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;

namespace DSharpPlus.CommandAll.Commands.Checks
{
    public class RequirePermissionsCheck : CommandCheckAttribute
    {
        public CommandCheckAttribute GuildCheck { get; init; } = new RequireGuildCheck();
        private readonly PermissionCheckType _type;
        private readonly Permissions _permissions;

        public RequirePermissionsCheck(PermissionCheckType type, Permissions permissions)
        {
            _type = type;
            _permissions = permissions;
        }

        [SuppressMessage("Roslyn", "IDE0046", Justification = "Readability")]
        public override async Task<bool> CanExecuteAsync(CommandContext context)
        {
            if (await GuildCheck.CanExecuteAsync(context))
            {
                return false;
            }

            if (_type.HasFlag(PermissionCheckType.User) && !context.Member!.Permissions.HasPermission(_permissions))
            {
                return false;
            }

            if (_type.HasFlag(PermissionCheckType.Bot) && !context.Guild!.CurrentMember.Permissions.HasPermission(_permissions))
            {
                return false;
            }

            return true;
        }
    }

    [Flags]
    public enum PermissionCheckType
    {
        User,
        Bot
    }
}
