using System;

namespace OoLunar.DSharpPlus.CommandAll.Attributes
{
    [AttributeUsage(AttributeTargets.Constructor | AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter, Inherited = false, AllowMultiple = false)]
    public sealed class NoDependencyInjectionAttribute : Attribute { }
}
