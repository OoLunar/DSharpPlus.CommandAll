using System.Threading;
using System.Threading.Tasks;
using DSharpPlus.CommandAll.Commands.Enums;

namespace DSharpPlus.CommandAll.Commands.Checks
{
    public class RequireReplyCheck : CommandCheckAttribute
    {
        private readonly CommandInvocationType AllowedInvocationTypes;

        public RequireReplyCheck(CommandInvocationType allowedInvocationTypes = CommandInvocationType.TextCommand) => AllowedInvocationTypes = allowedInvocationTypes | CommandInvocationType.TextCommand;

        public override Task<bool> CanExecuteAsync(CommandContext context, CancellationToken cancellationToken = default)
            => Task.FromResult((context.InvocationType & AllowedInvocationTypes) != 0 && context.Message!.ReferencedMessage is not null);
    }
}
