using System;

namespace OoLunar.DSharpPlus.CommandAll.Exceptions
{
    public sealed class HandlerNotImplementedException : CommandAllException
    {
        internal HandlerNotImplementedException(string message, Exception innerException) : base(message, innerException) { }
    }
}
