namespace OoLunar.DSharpPlus.CommandAll.Exceptions
{
    public sealed class PropertyOutOfRangeException : CommandAllException
    {
        internal PropertyOutOfRangeException(string? propertyName, object? minValue, object? maxValue, object? actualValue) : base($"Property '{propertyName}' does not have a value/count inside of a valid range! The expected range was between {minValue} and {maxValue}, but the actual value was {actualValue}!") { }
    }
}
