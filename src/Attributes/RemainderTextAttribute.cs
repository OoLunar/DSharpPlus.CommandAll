using System;

namespace OoLunar.DSharpPlus.CommandAll.Attributes
{
    /// <summary>
    /// A marker attribute that indicates that the parameter should take the rest of the input.
    /// </summary>
    [AttributeUsage(AttributeTargets.Parameter)]
    public sealed class RemainderTextAttribute : Attribute
    {
        /// <summary>
        /// Creates a new instance of <see cref="RemainderTextAttribute"/>.
        /// </summary>
        public RemainderTextAttribute() { }
    }
}
