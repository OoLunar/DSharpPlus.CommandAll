using System;

namespace OoLunar.DSharpPlus.CommandAll.Attributes
{
    /// <summary>
    /// Marks a class or method as a (sub)command or group.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, Inherited = true, AllowMultiple = false)]
    public sealed class CommandAttribute : Attribute
    {
        /// <summary>
        /// The name of the command.
        /// </summary>
        public readonly string Name;

        /// <summary>
        /// Aliases for the command.
        /// </summary>
        public readonly string[] Aliases;

        /// <summary>
        /// Creates a new command attribute.
        /// </summary>
        /// <param name="name">The name used to identify the command.</param>
        /// <param name="aliases">Aliases used to link back to the command.</param>
        public CommandAttribute(string name, params string[] aliases)
        {
            Name = name;
            Aliases = aliases;
        }
    }
}
