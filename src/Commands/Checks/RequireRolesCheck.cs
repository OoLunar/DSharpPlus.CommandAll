using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;

namespace DSharpPlus.CommandAll.Commands.Checks
{
    public class RequireRolesCheck<T> : CommandCheckAttribute where T : IRoleProvider
    {
        public RequireGuildCheck GuildCheck { get; init; } = new RequireGuildCheck();

        public override async Task<bool> CanExecuteAsync(CommandContext context)
        {
            if (!await GuildCheck.CanExecuteAsync(context))
            {
                return false;
            }

            T roleProvider = ActivatorUtilities.GetServiceOrCreateInstance<T>(context.Extension.ServiceProvider);
            return context.Member!.Roles.Select(role => role.Id).Intersect(await roleProvider.GetRolesAsync(context)).Any();
        }
    }

    public interface IRoleProvider
    {
        Task<IEnumerable<ulong>> GetRolesAsync(CommandContext context);
    }
}
