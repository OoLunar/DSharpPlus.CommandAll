using System;
using System.Collections.Generic;
using DSharpPlus.CommandAll.Commands.Checks;

namespace DSharpPlus.CommandAll.Exceptions
{
    public sealed class CommandChecksFailedException : Exception
    {
        public IReadOnlyList<CommandCheckResult> FailedChecks { get; init; }
        public CommandChecksFailedException(string message, IList<CommandCheckResult> failedChecks) : base(message) => FailedChecks = failedChecks.AsReadOnly() ?? throw new ArgumentNullException(nameof(failedChecks));
    }

    public sealed record CommandCheckResult
    {
        public CommandCheckAttribute Check { get; init; }
        public Exception? Exception { get; init; }
        public bool Success { get; init; }

        public CommandCheckResult(CommandCheckAttribute check, bool success, Exception? exception = null)
        {
            Check = check ?? throw new ArgumentNullException(nameof(check));
            if (success && exception is not null)
            {
                throw new ArgumentException("Cannot have a successful check with an exception.", nameof(exception));
            }

            Success = success;
            Exception = exception;
        }
    }
}
