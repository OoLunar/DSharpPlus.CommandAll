using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;

namespace DSharpPlus.CommandAll.Commands.Checks
{
    public class RequireRolesCheckAttribute<T> : CommandCheckAttribute where T : IRoleProvider
    {
        public RequireGuildCheckAttribute GuildCheck { get; init; } = new RequireGuildCheckAttribute();

        public override async Task<bool> CanExecuteAsync(CommandContext context, CancellationToken cancellationToken = default)
        {
            if (!await GuildCheck.CanExecuteAsync(context, cancellationToken))
            {
                return false;
            }

            T roleProvider = ActivatorUtilities.GetServiceOrCreateInstance<T>(context.Extension.ServiceProvider);
            return !cancellationToken.IsCancellationRequested && context.Member!.Roles
                .Select(role => role.Id)
                .Intersect(await roleProvider.GetRolesAsync(context, cancellationToken))
                .Any();
        }
    }

    public interface IRoleProvider
    {
        Task<IEnumerable<ulong>> GetRolesAsync(CommandContext context, CancellationToken cancellationToken = default);
    }
}
