using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using DSharpPlus.CommandAll.Commands.Converters;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace DSharpPlus.CommandAll.Managers
{
    /// <inheritdoc cref="IArgumentConverterManager" />
    public class ArgumentConverterManager : IArgumentConverterManager
    {
        private readonly Dictionary<Type, ArgumentConverterDefinition> _typeConverters = new();

        /// <summary>
        /// The service provider to use to create singleton instances of argument converters.
        /// </summary>
        private readonly IServiceCollection _serviceCollection;

        /// <summary>
        /// Used to log when a type isn't an argument converter or a parameter cannot be assigned an argument converter.
        /// </summary>
        private readonly ILogger<ArgumentConverterManager> _logger;

        /// <summary>
        /// Creates a new instance of <see cref="ArgumentConverterManager"/>.
        /// </summary>
        /// <param name="serviceCollection">The service provider to use to create singleton instances of argument converters.</param>
        /// <param name="logger">The logger to use to overly complain about things.</param>
        public ArgumentConverterManager(IServiceCollection serviceCollection, ILogger<ArgumentConverterManager> logger)
        {
            _serviceCollection = serviceCollection ?? throw new ArgumentNullException(nameof(serviceCollection));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <inheritdoc />
        public IReadOnlyDictionary<Type, ArgumentConverterDefinition> GetTypeConverters() => _typeConverters.AsReadOnly();

        /// <inheritdoc />
        public bool TryGetConverter(Type type, out ArgumentConverterDefinition? converter) => _typeConverters.TryGetValue(type, out converter);

        /// <inheritdoc />
        public IReadOnlyList<ArgumentConverterDefinition> AddArgumentConverters(Assembly assembly) => AddArgumentConverters(assembly.ExportedTypes.Where(type => !type.IsAbstract && !type.IsInterface && typeof(IArgumentConverter).IsAssignableFrom(type)).ToArray());

        /// <inheritdoc />
        public IReadOnlyList<ArgumentConverterDefinition> AddArgumentConverters(params Type[] types)
        {
            List<ArgumentConverterDefinition> addedConverters = new();

            // Create a service provider to access singleton instances of the argument converters.
            ServiceProvider serviceProvider = _serviceCollection.BuildServiceProvider();

            foreach (Type type in types)
            {
                if (type.IsAbstract || type.IsInterface)
                {
                    _logger.LogWarning("The type {Type} is abstract or an interface.", type);
                    continue;
                }
                else if (!typeof(IArgumentConverter).IsAssignableFrom(type))
                {
                    // The type does not inherit from IArgumentConverter.
                    _logger.LogWarning("The type {Type} is not assignable from {AssignableType}.", type, typeof(IArgumentConverter));
                    continue;
                }
                else if (_typeConverters.TryGetValue(type, out ArgumentConverterDefinition? existingConverter))
                {
                    _logger.LogWarning("Failed to register type {Type} as it is already handled by {ExistingConverter}.", type, existingConverter);
                    continue;
                }

                // Attempt to create a singleton instance of the argument converter.
                IArgumentConverter? instance = null;

                // Iterate over all public constructors
                foreach (ConstructorInfo constructor in type.GetConstructors(BindingFlags.Public | BindingFlags.Instance))
                {
                    // Get the constructor's parameters.
                    ParameterInfo[] parameters = constructor.GetParameters();
                    object?[] parameterInstances = new object[parameters.Length];

                    // Iterate over all parameters, searching for a singleton service that can be assigned to the parameter.
                    for (int i = 0; i < parameters.Length; i++)
                    {
                        ParameterInfo parameter = parameters[i];

                        // Attempt to find a singleton service that can be assigned to the parameter.
                        if (_serviceCollection.Any(service => service.Lifetime == ServiceLifetime.Singleton && parameter.ParameterType.IsGenericType ? parameter.ParameterType.GetGenericTypeDefinition() == service.ServiceType : parameter.ParameterType.IsAssignableFrom(service.ServiceType)))
                        {
                            // If a singleton service was found, get an instance of it and assign it to the parameter.
                            parameterInstances[i] = serviceProvider.GetRequiredService(parameter.ParameterType);
                        }
                        else if (parameter.HasDefaultValue)
                        {
                            // If no singleton service was found, check if the parameter has a default value.
                            parameterInstances[i] = parameter.DefaultValue;
                        }
                        else
                        {
                            // If no singleton service was found and the parameter doesn't have a default value, the constructor cannot be invoked.
                            break;
                        }
                    }

                    try
                    {
                        // Attempt to invoke the constructor with the parameter instances.
                        instance = constructor.Invoke(parameterInstances) as IArgumentConverter;
                    }
                    catch (Exception)
                    {
                        instance = null;
                    }
                }

                ArgumentConverterDefinition converter = new(type, type.GetGenericArguments(), instance);
                addedConverters.Add(converter);
                _typeConverters.Add(converter.GetOrCreateConverter(serviceProvider).Type, converter);
            }

            return addedConverters.AsReadOnly();
        }
    }
}
