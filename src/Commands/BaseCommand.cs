using System;
using System.Threading.Tasks;
using DSharpPlus.CommandAll.Exceptions;

namespace DSharpPlus.CommandAll.Commands
{
    /// <summary>
    /// A marker class used to indicate that this type is a top level command.
    /// </summary>
    /// <remarks>
    /// Nested classes should NOT inherit from this class.
    /// </remarks>
    public abstract class BaseCommand
    {
        /// <summary>
        /// This method is executed before the command overload is executed. If this method throws, the command overload will not be executed but instead <see cref="OnErrorAsync(CommandContext, Exception)"/> will be executed.
        /// </summary>
        public virtual Task BeforeExecutionAsync(CommandContext context) => Task.CompletedTask;

        /// <summary>
        /// This method is executed after the command overload is executed. If this method throws, <see cref="OnErrorAsync(CommandContext, Exception)"/> will be executed.
        /// </summary>
        public virtual Task AfterExecutionAsync(CommandContext context) => Task.CompletedTask;

        /// <summary>
        /// This method is executed when an exception is thrown during the execution of any of the execution methods or the main command overload. This method should be implemented when you still need to access the object's properties or fields during error handling.
        /// </summary>
        /// <remarks>
        /// If this method throws, the exception will be passed onto <see cref="CommandAllExtension.CommandErrored"/>. If that error handler throws, the exception will be logged.
        /// </remarks>
        public virtual Task OnErrorAsync(CommandContext context, Exception exception) => Task.FromException(new HandlerNotImplementedException("No error handler was provided for this command.", exception));
    }
}
