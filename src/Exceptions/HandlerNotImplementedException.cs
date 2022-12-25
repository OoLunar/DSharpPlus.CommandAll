using System;
using DSharpPlus.CommandAll.Commands;

namespace DSharpPlus.CommandAll.Exceptions
{
    /// <summary>
    /// Thrown when a method in <see cref="CommandContext"/> is not implemented.
    /// </summary>
    public sealed class HandlerNotImplementedException : CommandAllException
    {
        /// <summary>
        /// Creates a new <see cref="HandlerNotImplementedException"/>.
        /// </summary>
        internal HandlerNotImplementedException(string message, Exception innerException) : base(message, innerException) { }
    }
}
