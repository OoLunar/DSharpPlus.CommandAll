using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using DSharpPlus.CommandAll.Commands.Builders;
using DSharpPlus.CommandAll.Converters;
using Microsoft.Extensions.Logging;

namespace DSharpPlus.CommandAll.Managers
{
    /// <inheritdoc cref="IArgumentConverterManager" />
    public class ArgumentConverterManager : IArgumentConverterManager
    {
        /// <inheritdoc />
        public IReadOnlyDictionary<Type, Type> TypeConverters => _typeConverters.AsReadOnly();
        private readonly Dictionary<Type, Type> _typeConverters = new();

        /// <summary>
        /// Used to log when a type isn't an argument converter or a parameter cannot be assigned an argument converter.
        /// </summary>
        private readonly ILogger<ArgumentConverterManager> _logger;

        /// <summary>
        /// Creates a new instance of <see cref="ArgumentConverterManager"/>.
        /// </summary>
        /// <param name="logger">The logger to use to overly complain about things.</param>
        public ArgumentConverterManager(ILogger<ArgumentConverterManager>? logger = null) => _logger = logger ?? throw new ArgumentNullException(nameof(logger));

        /// <inheritdoc />
        public void AddArgumentConverter<T>() where T : IArgumentConverter => AddArgumentConverter(typeof(T));

        /// <inheritdoc />
        public void AddArgumentConverter(Type type) => AddArgumentConverters(new Type[] { type });

        /// <inheritdoc />
        public void AddArgumentConverters(Assembly assembly) => AddArgumentConverters(assembly.ExportedTypes);

        /// <inheritdoc />
        public void AddArgumentConverters(IEnumerable<Type> types)
        {
            foreach (Type type in types)
            {
                // Test if the type inherits from IArgumentConverter<T>
                Type? argumentInterface = type.GetInterfaces().FirstOrDefault(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IArgumentConverter<>));
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

        /// <inheritdoc />
        public bool TrySaturateParameters(IEnumerable<CommandParameterBuilder> parameters, [NotNullWhen(false)] out IEnumerable<CommandParameterBuilder> failedParameters)
        {
            List<CommandParameterBuilder> failed = new();
            foreach (CommandParameterBuilder parameter in parameters)
            {
                // Parameter does not have an argument converter.
                if (parameter.ArgumentConverterType is null)
                {
                    // Try finding a default type converter for the parameter type.
                    Type parameterType = Nullable.GetUnderlyingType(parameter.ParameterInfo!.ParameterType) ?? parameter.ParameterInfo.ParameterType;
                    if (!_typeConverters.TryGetValue(parameterType, out Type? converterType) && (!parameterType.IsArray || !_typeConverters.TryGetValue(parameterType.GetElementType()!, out converterType)))
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
