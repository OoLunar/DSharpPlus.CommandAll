using System;

namespace DSharpPlus.CommandAll.Commands.Enums
{
    /// <summary>
    /// Flags for command parameters.
    /// </summary>
    [Flags]
    public enum CommandParameterFlags
    {
        /// <summary>
        /// No flags.
        /// </summary>
        None = 0,

        /// <summary>
        /// This parameter is optional and has a default value.
        /// </summary>
        Optional = 1 << 1,

        /// <summary>
        /// This parameter is the last parameter and can take the rest of the input through an array.
        /// </summary>
        Params = 1 << 2,

        /// <summary>
        /// This parameter is a <see cref="string"/> and can take the rest of the input.
        /// </summary>
        RemainingText = 1 << 3,

        /// <summary>
        /// This flag is directly tied with <see langword="params"/>. If this flag is set, the parameter will trim the element's default value from the array.
        /// </summary>
        TrimExcess = 1 << 4,
    }
}
