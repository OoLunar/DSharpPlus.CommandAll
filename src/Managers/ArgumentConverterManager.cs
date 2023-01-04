using System;
using System.Collections;
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
        /// <summary>
        /// The list of registered argument converters.
        /// </summary>
        private readonly List<ArgumentConverterDefinition> _converters = new();

        /// <summary>
        /// The cache of argument converters that can convert a given type.
        /// </summary>
        private readonly Dictionary<Type, List<ArgumentConverterDefinition>> _typeConverterCache = new();

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
            _serviceCollection.AddSingleton<IArgumentConverterManager>(this);
            _serviceCollection.AddSingleton(serviceProvider => serviceProvider);
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <inheritdoc />
        public IReadOnlyList<ArgumentConverterDefinition> GetConverters<T>() => GetConverters(typeof(T));

        /// <inheritdoc />
        public IReadOnlyList<ArgumentConverterDefinition> GetConverters(Type? type = null)
        {

            // If the type is not specified, return all the converters available.
            if (type is null)
            {
                return _converters.AsReadOnly();
            }

            if (type.IsArray)
            {
                // If the type is an array, return all the converters that can convert the array's type.
                return GetConverters(type.GetElementType());
            }
            else if (type.IsAssignableFrom(typeof(IEnumerable)))
            {
                // If the type is an enumerable, return all the converters that can convert the enumerable's type.
                return GetConverters(type.GetGenericArguments().FirstOrDefault());
            }

            // Ensure the type is fully closed (I.E, Nullable<> is not okay, but Nullable<int> is fine).
            if (type.IsGenericTypeDefinition)
            {
                throw new ArgumentException("Cannot convert open generic types. Instead, please pass in a known generic type.", nameof(type));
            }
            // Check if the type if we've already cached the converters for it.
            else if (_typeConverterCache.TryGetValue(type, out List<ArgumentConverterDefinition>? cachedConverters))
            {
                // Return the cached converters.
                return cachedConverters.AsReadOnly();
            }
            // Cache the converters for a faster lookup next time.
            else
            {
                // Iterate through the converters and add them to the cache if they can convert the type.
                cachedConverters = _converters.Where(converter => converter.CanConvert(type)).ToList();

                // Add the cached converters to the dictionary.
                _typeConverterCache.Add(type, cachedConverters);

                // Return the cached converters.
                return cachedConverters.AsReadOnly();
            }
        }

        /// <inheritdoc />
        public IReadOnlyList<ArgumentConverterDefinition> AddArgumentConverters(Assembly assembly) => AddArgumentConverters(assembly.ExportedTypes.Where(type => !type.IsAbstract && !type.IsInterface && typeof(IArgumentConverter).IsAssignableFrom(type)).OrderBy(type => !type.IsGenericTypeDefinition).ToArray());

        /// <inheritdoc />
        public IReadOnlyList<ArgumentConverterDefinition> AddArgumentConverters(params Type[] types)
        {
            List<ArgumentConverterDefinition> addedConverters = new();

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

                ArgumentConverterDefinition converter = new(type, type.GetInterface("IArgumentConverter`1")?.GetGenericArguments() ?? Array.Empty<Type>(), _serviceCollection);
                addedConverters.Add(converter);
                _converters.Add(converter);
            }

            return addedConverters.AsReadOnly();
        }
    }
}
