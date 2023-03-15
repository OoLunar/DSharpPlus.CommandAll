using System;

namespace DSharpPlus.CommandAll.Attributes
{
    /// <summary>
    /// Determines the minimum and maximum values that a parameter can accept.
    /// </summary>
    [AttributeUsage(AttributeTargets.Parameter)]
    public sealed class ParameterLimitAttribute : Attribute
    {
        /// <summary>
        /// The minimum value that this parameter can accept.
        /// </summary>
        public int MinimumElementCount { get; init; }

        /// <summary>
        /// The maximum value that this parameter can accept.
        /// </summary>
        public int MaximumElementCount { get; init; }

        /// <summary>
        /// Whether or not to trim the excess elements from the parameter.
        /// </summary>
        public bool TrimExcess { get; init; } = true;

        /// <summary>
        /// Determines the minimum and maximum values that a parameter can accept.
        /// </summary>
        public ParameterLimitAttribute(int minimum = 0, int maximum = 25, bool trimExcess = true)
        {
            if (minimum < 0)
            {
                throw new ArgumentException("The minimum value cannot be less than 1.");
            }
            else if (maximum > 25)
            {
                throw new ArgumentException("The maximum value cannot be greater than 25.");
            }
            else if (minimum > maximum)
            {
                throw new ArgumentException("The minimum value cannot be greater than the maximum value.");
            }

            MinimumElementCount = minimum;
            MaximumElementCount = maximum;
            TrimExcess = trimExcess;
        }
    }
}
