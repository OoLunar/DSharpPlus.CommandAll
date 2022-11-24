using System;

namespace OoLunar.DSharpPlus.CommandAll.Exceptions
{
    public class CommandAllException : Exception
    {
        /// <inheritdoc cref="Exception(string?)"/>
        public CommandAllException(string message) : base(message) { }
    }
}
