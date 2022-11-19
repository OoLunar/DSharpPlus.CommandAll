using System;
using System.Threading.Tasks;

namespace OoLunar.DSharpPlus.CommandAll.Commands
{
    public abstract class BaseCommand
    {
        public virtual Task BeforeExecutionAsync(CommandContext context) => Task.CompletedTask;

        public virtual Task AfterExecutionAsync(CommandContext context) => Task.CompletedTask;

        public virtual Task OnErrorAsync(CommandContext context, AggregateException exception) => Task.CompletedTask;
    }
}
