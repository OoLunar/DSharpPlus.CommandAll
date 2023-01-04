using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;

namespace DSharpPlus.CommandAll.Commands.Converters
{
    /// <summary>
    /// The argument converter class exposed to the user.
    /// </summary>
    public sealed record ArgumentConverterDefinition
    {
        public Type ArgumentConverterType { get; }
        public IReadOnlyList<Type> GenericArguments { get; }
        public IReadOnlyDictionary<Type, object> SingletonInstances => _singletonInstances.AsReadOnly();
        private readonly Dictionary<Type, object> _singletonInstances = new();
        private readonly Func<Type, bool>? CanConvertDelegate;
        private readonly IServiceCollection _serviceCollection;
        private readonly IServiceProvider _serviceProvider;

        public ArgumentConverterDefinition(Type argumentConverterType, IReadOnlyList<Type> genericArguments, IServiceCollection serviceCollection)
        {
            ArgumentConverterType = argumentConverterType ?? throw new ArgumentNullException(nameof(argumentConverterType));
            GenericArguments = genericArguments ?? throw new ArgumentNullException(nameof(genericArguments));
            _serviceCollection = serviceCollection ?? throw new ArgumentNullException(nameof(serviceCollection));
            _serviceProvider = serviceCollection.BuildServiceProvider();

            // If the type is an open generic type, set the CanConvertMethod (as we need to make the generic version of the method).
            if (!argumentConverterType.IsGenericTypeDefinition)
            {
                if (TryCreateSingletonInstance(argumentConverterType, out IArgumentConverter? singleton))
                {
                    CanConvertDelegate = (argumentConverterType.GetMethod(nameof(IArgumentConverter.CanConvert), BindingFlags.Public | BindingFlags.Instance) ?? throw new ArgumentException("The type does not have a static method named CanConvert that takes a single Type parameter. This should be a compile time error.", nameof(argumentConverterType))).CreateDelegate(typeof(Func<Type, bool>), singleton) as Func<Type, bool>;
                    _singletonInstances.Add(argumentConverterType, singleton);
                }
            }
        }

        public bool CanConvert(Type type)
        {
            // If the type is not an open generic type, invoke the CanConvert delegate that was created in the constructor.
            if (!ArgumentConverterType.IsGenericTypeDefinition)
            {
                // This shouldn't be null, but if it is, throw an exception.
                return CanConvertDelegate is null
                    ? throw new InvalidOperationException("The type does not have a static method named CanConvert that takes a single Type parameter. This should be a compile time error.")
                    : CanConvertDelegate(type);
            }
            // The type is an open generic type, so we need to make the generic version of the method.
            else
            {
                // Create the generic type.
                Type generifiedType = ArgumentConverterType.MakeGenericType(type);

                // Grab the CanConvert method
                MethodInfo? canConvertMethod = generifiedType.GetMethod(nameof(IArgumentConverter.CanConvert), BindingFlags.Public | BindingFlags.Instance);

                IArgumentConverter? instance = GetOrCreateConverter(_serviceProvider, type);

                // The method should never be null, but if it is, throw an exception.
                return canConvertMethod is null
                    ? throw new InvalidOperationException("The type does not have a static method named CanConvert that takes a single Type parameter. This should be a compile time error.")
                    : (bool)canConvertMethod.Invoke(instance, new object[] { type })!;
            }
        }

        public IArgumentConverter GetOrCreateConverter(IServiceProvider serviceProvider, Type? type = null)
        {
            // Ensure the service provider is not null.
            if (serviceProvider is null)
            {
                throw new ArgumentNullException(nameof(serviceProvider));
            }
            // If the type is not an open generic type, attempt to get the singleton instance. If none exists, create a new instance with the service provider.
            else if (!ArgumentConverterType.IsGenericTypeDefinition)
            {
                return (IArgumentConverter)(_singletonInstances.TryGetValue(ArgumentConverterType, out object? converterObject)
                    ? converterObject!
                    : ActivatorUtilities.CreateInstance(serviceProvider, ArgumentConverterType));
            }
            else if (type is null)
            {
                throw new ArgumentNullException(nameof(type));
            }
            // Create the generic type.
            else
            {
                if (_singletonInstances.TryGetValue(type, out object? converterObject))
                {
                    // Return the singleton instance for this specific set of generic arguments.
                    return (IArgumentConverter)converterObject!;
                }

                // Create the generic type.
                Type generifiedType = ArgumentConverterType.MakeGenericType(type);

                // Create a new instance with the service provider.
                if (TryCreateSingletonInstance(generifiedType, out IArgumentConverter? converter))
                {
                    // Add the instance to the singleton instances dictionary.
                    _singletonInstances.Add(type, converter);
                }
                else
                {
                    // The type requires scoped or transient services, so create a new instance with the service provider.
                    converter = (IArgumentConverter)ActivatorUtilities.CreateInstance(serviceProvider, generifiedType);
                }

                return converter;
            }
        }

        private bool TryCreateSingletonInstance(Type type, [NotNullWhen(true)] out IArgumentConverter? converter)
        {
            converter = null;

            // Iterate over all public constructors
            foreach (ConstructorInfo constructor in type.GetConstructors(BindingFlags.Public | BindingFlags.Instance))
            {
                // Get the constructor's parameters.
                ParameterInfo[] parameters = constructor.GetParameters();
                List<object?> parameterInstances = new();

                // Iterate over all parameters, searching for a singleton service that can be assigned to the parameter.
                for (int i = 0; i < parameters.Length; i++)
                {
                    ParameterInfo parameter = parameters[i];

                    // Attempt to find a singleton service that can be assigned to the parameter.
                    if (_serviceCollection.Any(service => service.Lifetime == ServiceLifetime.Singleton && parameter.ParameterType.IsGenericType ? parameter.ParameterType.GetGenericTypeDefinition() == service.ServiceType : parameter.ParameterType.IsAssignableFrom(service.ServiceType)))
                    {
                        // If a singleton service was found, get an instance of it and assign it to the parameter.
                        parameterInstances.Add(_serviceProvider.GetRequiredService(parameter.ParameterType));
                    }
                    else if (parameter.HasDefaultValue)
                    {
                        // If no singleton service was found, check if the parameter has a default value.
                        parameterInstances.Add(parameter.DefaultValue);
                    }
                    else
                    {
                        // If no singleton service was found and the parameter doesn't have a default value, the constructor cannot be invoked.
                        break;
                    }
                }

                // If the parameter instances list doesn't match the number of parameters, the constructor cannot be invoked.
                if (parameterInstances.Count != parameters.Length)
                {
                    continue;
                }

                try
                {
                    // Attempt to invoke the constructor with the parameter instances.
                    converter = constructor.Invoke(parameterInstances.ToArray()) as IArgumentConverter;
                }
                catch (Exception)
                {
                    converter = null;
                }
            }

            return converter is not null;
        }
    }
}
