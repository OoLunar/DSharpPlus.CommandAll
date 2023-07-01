using System;
using DSharpPlus.CommandAll.Commands.Enums;

namespace DSharpPlus.CommandAll.Attributes
{
    /// <summary>
    /// When a parameter is marked as nullable, this will determine which conditions the parameter is not null in.
    /// </summary>
    [AttributeUsage(AttributeTargets.Parameter, AllowMultiple = false, Inherited = true)]
    public sealed class RequiresCommandTypeAttribute : Attribute
    {
        /// <summary>
        /// The conditions the parameter is not null in.
        /// </summary>
        public CommandInvocationType CommandType { get; init; }

        /// <summary>
        /// When a parameter is marked as nullable, this will determine which conditions the parameter is not null in.
        /// </summary>
        public RequiresCommandTypeAttribute(CommandInvocationType commandType) => CommandType = commandType;
    }
}
