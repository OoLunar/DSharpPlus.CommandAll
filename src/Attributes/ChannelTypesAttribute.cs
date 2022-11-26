using System;
using System.Collections.Generic;
using DSharpPlus;

namespace OoLunar.DSharpPlus.CommandAll.Attributes
{
    [AttributeUsage(AttributeTargets.Parameter, AllowMultiple = false, Inherited = true)]
    public sealed class ChannelTypesAttribute : Attribute
    {
        public IReadOnlyList<ChannelType> ChannelTypes { get; }

        public ChannelTypesAttribute(params ChannelType[] channelTypes) => ChannelTypes = channelTypes.Length == 0 ? throw new ArgumentOutOfRangeException(nameof(channelTypes), "ChannelTypes must have at least one value!") : channelTypes;
    }
}
