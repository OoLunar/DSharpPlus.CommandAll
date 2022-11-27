using System;
using System.Diagnostics.CodeAnalysis;

namespace OoLunar.DSharpPlus.CommandAll.Commands.Builders
{
    /// <summary>
    /// A pattern used to build objects.
    /// </summary>
    public interface IBuilder
    {
        // TODO: Parse and TryParse methods?

        /// <summary>
        /// Attempts to verify the builder's data, throwing an exception if it fails.
        /// </summary>
        void Verify();

        /// <summary>
        /// Attempts to verify the builder's data, returning whether it succeeded.
        /// </summary>
        bool TryVerify();

        /// <summary>
        /// Attempts to verify the builder's data, returning whether it succeeded.
        /// </summary>
        /// <param name="exception">The exception that was thrown, if any.</param>
        bool TryVerify([NotNullWhen(false)] out Exception? error);
    }
}
