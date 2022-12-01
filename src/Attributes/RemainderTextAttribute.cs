using System;

namespace OoLunar.DSharpPlus.CommandAll.Attributes
{
    /// <summary>
    /// A marker attribute that indicates that the parameter should take the rest of the input.
    /// </summary>
    [AttributeUsage(AttributeTargets.Parameter)]
    public sealed class RemainingTextAttribute : Attribute
    {
        /// <summary>
        /// Creates a new instance of <see cref="RemainingTextAttribute"/>.
        /// </summary>
        public RemainingTextAttribute() { }
    }
}
