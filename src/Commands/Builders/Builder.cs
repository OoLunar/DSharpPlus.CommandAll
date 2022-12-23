using System;
using System.Diagnostics.CodeAnalysis;

namespace DSharpPlus.CommandAll.Commands.Builders
{
    /// <summary>
    /// A pattern used to build objects.
    /// </summary>
    public abstract class Builder
    {
        // TODO: Parse and TryParse methods?

        /// <summary>
        /// The <see cref="CommandAllExtension"/> that's used for naming conventions.
        /// </summary>
        public readonly CommandAllExtension CommandAllExtension;

        /// <summary>
        /// Creates a new builder, with the given <see cref="CommandAllExtension"/>.
        /// </summary>
        /// <param name="commandAllExtension">The <see cref="CommandAllExtension"/> to use for naming.</param>
        public Builder(CommandAllExtension commandAllExtension) => CommandAllExtension = commandAllExtension ?? throw new ArgumentNullException(nameof(commandAllExtension));

        /// <summary>
        /// Attempts to verify the builder's data, throwing an exception if it fails.
        /// </summary>
        public abstract void Verify();

        /// <summary>
        /// Attempts to verify the builder's data, returning whether it succeeded.
        /// </summary>
        public abstract bool TryVerify();

        /// <summary>
        /// Attempts to verify the builder's data, returning whether it succeeded.
        /// </summary>
        /// <param name="exception">The exception that was thrown, if any.</param>
        public abstract bool TryVerify([NotNullWhen(false)] out Exception? error);
    }
}
