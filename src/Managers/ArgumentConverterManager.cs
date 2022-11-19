using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using OoLunar.DSharpPlus.CommandAll.Commands;
using OoLunar.DSharpPlus.CommandAll.Commands.Arguments;

namespace OoLunar.DSharpPlus.CommandAll.Managers
{
    public class ArgumentConverterManager : IArgumentConverterManager
    {
        private static readonly Type _converterType = typeof(IArgumentConverter<>);

        public IReadOnlyList<CommandParameter> Parameters => _parameters;
        public IReadOnlyDictionary<Type, Type> TypeConverters => _typeConverters;

        private readonly List<CommandParameter> _parameters = new();
        private readonly Dictionary<Type, Type> _typeConverters = new();
        private readonly ILogger<ArgumentConverterManager> _logger = NullLogger<ArgumentConverterManager>.Instance;

        public ArgumentConverterManager(ILogger<ArgumentConverterManager>? logger = null) => _logger = logger ?? NullLogger<ArgumentConverterManager>.Instance;

        public void AddArgumentConverter(Type type) => AddArgumentConverters(new Type[] { type });
        public void AddArgumentConverter<T>() where T : IArgumentConverter<T> => AddArgumentConverter(typeof(T));
        public void AddArgumentConverters(Assembly assembly) => AddArgumentConverters(assembly.ExportedTypes);
        public void AddArgumentConverters(IEnumerable<Type> types)
        {
            foreach (Type type in types)
            {
                // Test if the type inherits from IArgumentConverter<T>
                Type? argumentInterface = type.GetInterfaces().FirstOrDefault(i => i.IsGenericType && i.GetGenericTypeDefinition() == _converterType);
                if (argumentInterface is null)
                {
                    _logger.LogTrace("Type {type} does not inherit from IArgumentConverter<T>, skipping adding it as an argument converter.", type);
                    continue;
                }
                else if (_typeConverters.TryGetValue(type, out Type? existingType))
                {
                    _logger.LogWarning("Cannot register Argument Converter {ArgumentConverter} for type {Type} because Argument Converter {ExistingArgumentConverter} is already registered for type {Type}", type, type.GenericTypeArguments[0], existingType, type.GenericTypeArguments[0]);
                    continue;
                }

                _typeConverters.Add(argumentInterface.GenericTypeArguments[0], type);

                // Go through all the parameters and set the argument converter type if it is null
                foreach (CommandParameter parameter in _parameters)
                {
                    // TrySet sets the converter for us.
                    if (parameter.ArgumentConverterType is null && parameter.Type == type.GenericTypeArguments[0] && parameter.TrySetArgumentConverterType(type))
                    {
                        _logger.LogTrace("Set {ArgumentConverter} as the default argument converter for parameter {Parameter}", type, parameter);
                    }
                }
            }
        }

        public void AddParameters(IEnumerable<CommandParameter> parameters)
        {
            foreach (CommandParameter parameter in parameters)
            {
                // Parameter does not have an argument converter.
                if (parameter.ArgumentConverterType is null && !parameter.Type.IsAssignableFrom(typeof(CommandContext)))
                {
                    // Try finding a default type converter for the parameter type.
                    if (!_typeConverters.TryGetValue(parameter.Type, out Type? converterType))
                    {
                        throw new ArgumentException($"No Argument Converter is registered for type {parameter.Type}");
                    }
                    // Try setting the default type converter for the parameter.
                    else if (!parameter.TrySetArgumentConverterType(converterType, out string? error))
                    {
                        throw new ArgumentException($"Failed to set {converterType} as an argument converter for parameter {parameter.ParameterInfo.Member} {parameter.ParameterInfo.Name}: {error}");
                    }
                    // Argument converter was set successfully.
                    else
                    {
                        _logger.LogTrace("Set {ArgumentConverter} as the default argument converter for parameter {Parameter}", converterType, parameter);
                    }
                }

                // Try to register the parameter.
                if (parameters.Contains(parameter))
                {
                    _logger.LogWarning("Cannot register parameter {Parameter} again because it was already registered once before!", parameter);
                }

                // Parameter was registered successfully.
                _parameters.Add(parameter);
                _logger.LogTrace("Set {ArgumentConverter} as an argument converter for parameter {Parameter}", parameter.ArgumentConverterType, parameter);
            }
        }
    }
}
