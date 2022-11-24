namespace OoLunar.DSharpPlus.CommandAll.Exceptions
{
    public sealed class InvalidPropertyStateException : CommandAllException
    {
        internal InvalidPropertyStateException(string propertyName, string message) : base($"Property '{propertyName}' has an invalid state: {message}") { }
    }
}
