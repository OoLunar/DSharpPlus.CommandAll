using System;

namespace OoLunar.DSharpPlus.CommandAll.Commands.Enums
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
    }
}
