using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;

namespace DSharpPlus.CommandAll.Commands.Checks
{
    public class RequirePermissionsCheckAttribute : CommandCheckAttribute
    {
        public CommandCheckAttribute GuildCheck { get; init; } = new RequireGuildCheckAttribute();
        public PermissionCheckType PermissionType { get; init; }
        public Permissions Permissions { get; init; }

        public RequirePermissionsCheckAttribute(PermissionCheckType type, Permissions permissions)
        {
            PermissionType = type;
            Permissions = permissions;
        }

        [SuppressMessage("Roslyn", "IDE0046", Justification = "Readability")]
        public override async Task<bool> CanExecuteAsync(CommandContext context, CancellationToken cancellationToken = default)
        {
            if (!await GuildCheck.CanExecuteAsync(context, cancellationToken))
            {
                return false;
            }

            if (PermissionType.HasFlag(PermissionCheckType.User) && !context.Member!.Permissions.HasPermission(Permissions))
            {
                return false;
            }

            if (PermissionType.HasFlag(PermissionCheckType.Bot) && !context.Guild!.CurrentMember.Permissions.HasPermission(Permissions))
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
