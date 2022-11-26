using System;

namespace OoLunar.DSharpPlus.CommandAll.Attributes
{
    [AttributeUsage(AttributeTargets.Parameter)]
    public sealed class MinMaxAttribute : Attribute
    {
        public object? MinValue;
        public object? MaxValue;

        public MinMaxAttribute()
        {
            if (MinValue is int minInt && MaxValue is int maxInt && minInt > maxInt)
            {
                throw new ArgumentException("The minimum value cannot be greater than the maximum value.");
            }
            else if (MinValue is double minDouble && MaxValue is double maxDouble && minDouble > maxDouble)
            {
                throw new ArgumentException("The minimum value cannot be greater than the maximum value.");
            }
            else if (MinValue is not null && MaxValue is not null && MinValue.GetType() != MaxValue.GetType())
            {
                throw new ArgumentException("The minimum and maximum values must be of the same type.");
            }
        }
    }
}
