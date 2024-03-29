using System;
using System.Threading.Tasks;
using DSharpPlus.CommandAll.Commands;

namespace DSharpPlus.CommandAll.ContextChecks
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method | AttributeTargets.Delegate)]
    public class RequirePermissionsAttribute : RequireGuildAttribute
    {
        public Permissions BotPermissions { get; init; }
        public Permissions UserPermissions { get; init; }

        public RequirePermissionsAttribute(Permissions permissions) => BotPermissions = UserPermissions = permissions;
        public RequirePermissionsAttribute(Permissions botPermissions, Permissions userPermissions)
        {
            BotPermissions = botPermissions;
            UserPermissions = userPermissions;
        }

        public override async Task<bool> ExecuteCheckAsync(CommandContext context) => await base.ExecuteCheckAsync(context)
            && context.Guild!.CurrentMember.PermissionsIn(context.Channel).HasPermission(BotPermissions)
            && context.Member!.PermissionsIn(context.Channel).HasPermission(UserPermissions);
    }
}
