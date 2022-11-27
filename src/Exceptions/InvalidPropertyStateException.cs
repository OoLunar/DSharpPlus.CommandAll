namespace OoLunar.DSharpPlus.CommandAll.Exceptions
{
    /// <summary>
    /// Thrown when a property has an invalid value.
    /// </summary>
    public sealed class InvalidPropertyStateException : CommandAllException
    {
        /// <summary>
        /// Creates a new instance of <see cref="InvalidPropertyStateException"/>.
        /// </summary>
        /// <param name="propertyName">The problem property.</param>
        /// <param name="message">A message detailing why the value on the property is invalid.</param>
        internal InvalidPropertyStateException(string propertyName, string message) : base($"Property '{propertyName}' has an invalid state: {message}") { }
    }
}
