using System;

namespace DSharpPlus.CommandAll.Attributes
{
    /// <summary>
    /// Marks a parameter as supporting auto-completion.
    /// </summary>
    [AttributeUsage(AttributeTargets.Parameter, Inherited = true, AllowMultiple = false)]
    public sealed class AutoCompleteAttribute : Attribute { }
}
