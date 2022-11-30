using System;
using System.Linq;
using OoLunar.DSharpPlus.CommandAll.Commands.Arguments;

namespace OoLunar.DSharpPlus.CommandAll.Attributes
{
    /// <summary>
    /// Attempts to parse a parameter with the specified converter.
    /// </summary>
    [AttributeUsage(AttributeTargets.Parameter, Inherited = true, AllowMultiple = false)]
    public class ArgumentConverterAttribute : Attribute
    {
        /// <summary>
        /// The converter to use.
        /// </summary>
        public readonly Type ArgumentConverterType;

        /// <summary>
        /// Attempts to parse a parameter with the specified converter.
        /// </summary>
        /// <param name="converterType">The converter to use.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="converterType"/> is null.</exception>
        /// <exception cref="ArgumentException">Thrown when <paramref name="converterType"/> does not implement <see cref="IArgumentConverter"/>.</exception>
        public ArgumentConverterAttribute(Type parameterConverter)
        {
            if (parameterConverter is null)
            {
                throw new ArgumentNullException(nameof(parameterConverter));
            }
            else if (parameterConverter.GetInterfaces().FirstOrDefault(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IArgumentConverter<>)) is null)
            {
                throw new ArgumentException($"Converter type must implement IArgumentConverter<`1>.");
            }
            ArgumentConverterType = parameterConverter;
        }
    }

    /// <summary>
    /// Attempts to parse a parameter with the specified converter.
    /// </summary>
    public sealed class ArgumentConverterAttribute<T> : ArgumentConverterAttribute
    {
        /// <summary>
        /// Attempts to parse a parameter with the specified converter.
        /// </summary>
        public ArgumentConverterAttribute() : base(typeof(T)) { }
    }
}
