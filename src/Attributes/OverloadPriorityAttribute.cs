namespace OoLunar.DSharpPlus.CommandAll.Attributes
{
    /// <summary>
    /// Determines which overload should be picked first when multiple overloads are available. The overloads are sorted by the priority's highest number.
    /// </summary>
    /// <remarks>
    /// This attribute is only used when multiple overloads are available. If only one overload is available, it will be used regardless of this attribute.
    /// </remarks>
    [System.AttributeUsage(System.AttributeTargets.Method, Inherited = true, AllowMultiple = false)]
    public sealed class OverloadPriorityAttribute : System.Attribute
    {
        /// <summary>
        /// The higher the number, the more priority it has.
        /// </summary>
        public readonly int Priority;

        /// <summary>
        /// Determines which overload should be picked first when multiple overloads are available. The overloads are sorted by the priority's highest number.
        /// </summary>
        public OverloadPriorityAttribute(int priority) => Priority = priority;
    }
}
