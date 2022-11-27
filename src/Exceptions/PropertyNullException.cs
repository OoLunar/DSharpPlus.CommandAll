namespace OoLunar.DSharpPlus.CommandAll.Exceptions
{
    /// <summary>
    /// Thrown when a property has an unexpected null value.
    /// </summary>
    public sealed class PropertyNullException : CommandAllException
    {
        /// <summary>
        /// Creates a new instance of <see cref="PropertyNullException"/>.
        /// </summary>
        /// <param name="propertyName">The problem property.</param>
        internal PropertyNullException(string propertyName) : base($"Property '{propertyName}' is null or whitespace.") { }
    }
}
