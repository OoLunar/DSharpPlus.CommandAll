using System;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using OoLunar.DSharpPlus.CommandAll.Attributes;
using OoLunar.DSharpPlus.CommandAll.Commands.Arguments;

namespace OoLunar.DSharpPlus.CommandAll.Commands
{
    public sealed class CommandParameter
    {
        private static readonly Type _converterType = typeof(IArgumentConverter<>);

        public readonly string Name;
        public readonly string? Description;
        public readonly ParameterInfo ParameterInfo;
        public readonly CommandParameterFlags Flags;
        public readonly object? DefaultValue;
        public Type? ArgumentConverterType { get; private set; }
        public Type Type => ParameterInfo.ParameterType;

        public CommandParameter(ParameterInfo parameterInfo)
        {
            Name = parameterInfo.Name!;
            Description = parameterInfo.GetCustomAttribute<DescriptionAttribute>()?.Description;
            ParameterInfo = parameterInfo;
            DefaultValue = parameterInfo.DefaultValue;
            ArgumentConverterType = parameterInfo.GetCustomAttribute<ArgumentConverterAttribute>()?.ArgumentConverterType;

            if (ParameterInfo.IsOptional)
            {
                Flags |= CommandParameterFlags.Optional;
            }

            if (ParameterInfo.GetCustomAttribute<ParamArrayAttribute>() != null)
            {
                Flags |= CommandParameterFlags.Params;
            }
        }

        public void SetArgumentConverterType(Type argumentConverterType)
        {
            if (!TrySetArgumentConverterType(argumentConverterType, out string? error))
            {
                throw new ArgumentException(error);
            }
        }

        public bool TrySetArgumentConverterType(Type type) => TrySetArgumentConverterType(type, out _);

        public bool TrySetArgumentConverterType(Type type, out string? error)
        {
            if (type is null)
            {
                error = "Argument converter type is null";
                return false;
            }

            Type? argumentConverterInterface = type.GetInterfaces().FirstOrDefault(i => i.IsGenericType && i.GetGenericTypeDefinition() == _converterType);
            if (argumentConverterInterface is null)
            {
                error = "Argument converter type is not a subclass of IArgumentConverter<>";
                return false;
            }
            else if (argumentConverterInterface.GenericTypeArguments[0] != Type)
            {
                error = "Argument converter type is not a generic type of the parameter type";
                return false;
            }

            ArgumentConverterType = type;
            error = null;
            return true;
        }
    }
}
