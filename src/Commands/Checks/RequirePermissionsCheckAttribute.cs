using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;

namespace DSharpPlus.CommandAll.Commands.Checks
{
    public class RequirePermissionsCheckAttribute : CommandCheckAttribute
    {
        public CommandCheckAttribute GuildCheck { get; init; } = new RequireGuildCheckAttribute();
        private readonly PermissionCheckType _type;
        private readonly Permissions _permissions;

        public RequirePermissionsCheckAttribute(PermissionCheckType type, Permissions permissions)
        {
            _type = type;
            _permissions = permissions;
        }

        [SuppressMessage("Roslyn", "IDE0046", Justification = "Readability")]
        public override async Task<bool> CanExecuteAsync(CommandContext context, CancellationToken cancellationToken = default)
        {
            if (!await GuildCheck.CanExecuteAsync(context, cancellationToken))
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
