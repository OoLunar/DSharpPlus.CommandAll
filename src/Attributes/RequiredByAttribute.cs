using System;

namespace DSharpPlus.CommandAll.Attributes
{
    /// <summary>
    /// Specifies when a parameter isn't null.
    /// </summary>
    [Flags]
    public enum RequiredBy
    {
        /// <summary>
        /// The parameter will not be null when the command is a slash command.
        /// </summary>
        SlashCommand = 1 << 1,

        /// <summary>
        /// The parameter will not be null when the command is a text command.
        /// </summary>
        TextCommand = 1 << 2,
    }

    /// <summary>
    /// When a parameter is marked as nullable, this will determine which conditions the parameter is not null in.
    /// </summary>
    [AttributeUsage(AttributeTargets.Parameter, AllowMultiple = false, Inherited = true)]
    public sealed class RequiredByAttribute : Attribute
    {
        /// <summary>
        /// The conditions the parameter is not null in.
        /// </summary>
        public RequiredBy RequiredBy { get; init; }

        /// <summary>
        /// When a parameter is marked as nullable, this will determine which conditions the parameter is not null in.
        /// </summary>
        public RequiredByAttribute(RequiredBy requiredBy) => RequiredBy = requiredBy;
    }
}
