using System;

namespace OoLunar.DSharpPlus.CommandAll.Attributes
{
    [AttributeUsage(AttributeTargets.Parameter)]
    public sealed class MinMaxAttribute : Attribute
    {
        public readonly object? MinValue;
        public readonly object? MaxValue;

        public MinMaxAttribute(int? minValue, int? maxValue)
        {
            if (minValue is not null && maxValue is not null && minValue > maxValue)
            {
                throw new ArgumentException("The minimum value cannot be greater than the maximum value.");
            }

            MinValue = minValue;
            MaxValue = maxValue;
        }

        public MinMaxAttribute(double? minValue, double? maxValue)
        {
            if (minValue is not null && maxValue is not null && minValue > maxValue)
            {
                throw new ArgumentException("The minimum value cannot be greater than the maximum value.");
            }

            MinValue = minValue;
            MaxValue = maxValue;
        }
    }
}
