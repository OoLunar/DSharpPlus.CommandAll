using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using OoLunar.DSharpPlus.CommandAll.Commands.Arguments;
using OoLunar.DSharpPlus.CommandAll.Commands.Builders;

namespace OoLunar.DSharpPlus.CommandAll.Managers
{
    public class ArgumentConverterManager : IArgumentConverterManager
    {
        private static readonly Type _converterType = typeof(IArgumentConverter<>);

        public IReadOnlyDictionary<Type, Type> TypeConverters => _typeConverters;
        private readonly Dictionary<Type, Type> _typeConverters = new();

        private readonly ILogger<ArgumentConverterManager> _logger = NullLogger<ArgumentConverterManager>.Instance;

        public ArgumentConverterManager(ILogger<ArgumentConverterManager>? logger = null) => _logger = logger ?? NullLogger<ArgumentConverterManager>.Instance;

        public void AddArgumentConverter(Type type) => AddArgumentConverters(new Type[] { type });
        public void AddArgumentConverter<T>() where T : IArgumentConverter => AddArgumentConverter(typeof(T));
        public void AddArgumentConverters(Assembly assembly) => AddArgumentConverters(assembly.ExportedTypes);
        public void AddArgumentConverters(IEnumerable<Type> types)
        {
            foreach (Type type in types)
            {
                // Test if the type inherits from IArgumentConverter<T>
                Type? argumentInterface = type.GetInterfaces().FirstOrDefault(i => i.IsGenericType && i.GetGenericTypeDefinition() == _converterType);
                if (argumentInterface is null)
                {
                    _logger.LogDebug("Type {type} does not inherit from IArgumentConverter<T>, skipping adding it as an argument converter.", type);
                    continue;
                }
                else if (_typeConverters.TryGetValue(type, out Type? existingType))
                {
                    _logger.LogWarning("Cannot register Argument Converter {ArgumentConverter} for type {Type} because Argument Converter {ExistingArgumentConverter} is already registered for type {Type}", type, type.GenericTypeArguments[0], existingType, type.GenericTypeArguments[0]);
                    continue;
                }

                _typeConverters.Add(argumentInterface.GenericTypeArguments[0], type);
            }
        }

        public bool TrySaturateParameters(IEnumerable<CommandParameterBuilder> parameters, [NotNullWhen(false)] out IEnumerable<CommandParameterBuilder> failedParameters)
        {
            List<CommandParameterBuilder> failed = new();
            foreach (CommandParameterBuilder parameter in parameters)
            {
                // Parameter does not have an argument converter.
                if (parameter.ArgumentConverterType is null)
                {
                    // Try finding a default type converter for the parameter type.
                    if (!_typeConverters.TryGetValue(parameter.ParameterInfo!.ParameterType, out Type? converterType))
                    {
                        failed.Add(parameter);
                        _logger.LogTrace("Could not find an argument converter for parameter {Parameter}", parameter);
                        continue;
                    }
                    // Argument converter was set successfully.
                    else
                    {
                        parameter.ArgumentConverterType = converterType;
                        _logger.LogTrace("Set {ArgumentConverter} as the default argument converter for parameter {Parameter}", converterType, parameter);
                        continue;
                    }
                }
            }

            failedParameters = failed;
            return !failed.Any();
        }
    }
}
