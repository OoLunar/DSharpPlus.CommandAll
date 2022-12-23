using System;
using System.Collections.Generic;

namespace DSharpPlus.CommandAll.Attributes
{
    /// <summary>
    /// Determines the channel types that a parameter can accept.
    /// </summary>
    [AttributeUsage(AttributeTargets.Parameter, Inherited = true, AllowMultiple = false)]
    public sealed class ChannelTypesAttribute : Attribute
    {
        /// <summary>
        /// The acceptable channel types for this parameter.
        /// </summary>
        public IReadOnlyList<ChannelType> ChannelTypes { get; }

        /// <summary>
        /// Determines the channel types that a parameter can accept.
        /// </summary>
        /// <param name="channelTypes">The acceptable channel types for this parameter.</param>
        public ChannelTypesAttribute(params ChannelType[] channelTypes) => ChannelTypes = channelTypes.Length == 0 ? throw new ArgumentOutOfRangeException(nameof(channelTypes), "ChannelTypes must have at least one value!") : channelTypes;
    }
}
