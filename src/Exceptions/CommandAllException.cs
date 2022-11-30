using System;
using OoLunar.DSharpPlus.CommandAll.Commands.Builders;

namespace OoLunar.DSharpPlus.CommandAll.Exceptions
{
    /// <summary>
    /// Thrown when an exception occurs while parsing with any <see cref="Builder"/>.
    /// </summary>
    public class CommandAllException : Exception
    {
        /// <summary>
        /// Creates a new <see cref="CommandAllException"/>.
        /// </summary>
        /// <param name="message">The message to display.</param>
        internal CommandAllException(string message) : base(message) { }

        /// <summary>
        /// Creates a new <see cref="CommandAllException"/>.
        /// </summary>
        /// <param name="message">The message to display.</param>
        /// <param name="innerException">The inner exception.</param>
        internal CommandAllException(string message, Exception innerException) : base(message, innerException) { }
    }
}
