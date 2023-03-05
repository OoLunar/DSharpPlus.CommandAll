using System;
using System.Threading.Tasks;

namespace DSharpPlus.CommandAll.Commands.Checks
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true, Inherited = false)]
    public abstract class CommandCheckAttribute : Attribute
    {
        public abstract Task<bool> CanExecuteAsync(CommandContext context);
    }
}
