namespace OoLunar.DSharpPlus.CommandAll.Exceptions
{
    /// <summary>
    /// Thrown when a property has a value that did not fall between the expected range.
    /// </summary>
    public sealed class PropertyOutOfRangeException : CommandAllException
    {
        /// <summary>
        /// Creates a new instance of <see cref="PropertyOutOfRangeException"/>.
        /// </summary>
        /// <param name="propertyName">The problem property.</param>
        /// <param name="minValue">The minimum value.</param>
        /// <param name="maxValue">The maximum value.</param>
        /// <param name="actualValue">The actual value.</param>
        internal PropertyOutOfRangeException(string? propertyName, object? minValue, object? maxValue, object? actualValue) : base($"Property '{propertyName}' does not have a value/count inside of a valid range! The expected range was between {minValue} and {maxValue}, but the actual value was {actualValue}!") { }
    }
}
