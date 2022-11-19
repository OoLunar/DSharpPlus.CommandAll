using System;
using System.Collections.Generic;
using System.Reflection;
using OoLunar.DSharpPlus.CommandAll.Attributes;

namespace OoLunar.DSharpPlus.CommandAll
{
    public static class ReflectionUtilities
    {
        /// <summary>
        /// Returns a member info's name and, if available, fullname prepended.
        /// </summary>
        /// <param name="memberInfo">The <see cref="MemberInfo"/> to get the fullname of.</param>
        public static string GetFullname(MemberInfo memberInfo) => (memberInfo.DeclaringType == null ? null : memberInfo.DeclaringType.FullName + '.') + memberInfo.Name;

        public static object? InsertDependencyInjection(IServiceProvider serviceProvider, Type type)
        {
            // Constructor injection
            object? createdObject = null;
            object? service;
            foreach (ConstructorInfo constructor in type.GetConstructors(BindingFlags.Public))
            {
                if (constructor.GetCustomAttribute<NoDependencyInjectionAttribute>() is not null)
                {
                    continue;
                }

                List<object> constructorParameters = new();
                foreach (ParameterInfo parameter in constructor.GetParameters())
                {
                    if (parameter.GetCustomAttribute<NoDependencyInjectionAttribute>() is not null || (service = serviceProvider.GetService(parameter.ParameterType)) is null)
                    {
                        continue;
                    }

                    constructorParameters.Add(service);
                }

                createdObject = Activator.CreateInstance(type, constructorParameters);
            }

            // If no valid constructor was found.
            if (createdObject is null)
            {
                return null;
            }

            // Property injection, specific to D#+
            foreach (PropertyInfo property in type.GetProperties())
            {
                if (property.SetMethod?.IsPublic is null or false || property.GetCustomAttribute<NoDependencyInjectionAttribute>() is not null || (service = serviceProvider.GetService(property.PropertyType)) is null)
                {
                    continue;
                }

                property.SetValue(createdObject, service);
            }

            // Field injection, used for init only properties/readonly fields. Specific to this extension.
            foreach (FieldInfo field in type.GetFields())
            {
                if (field.IsPublic is false || field.GetCustomAttribute<NoDependencyInjectionAttribute>() is not null || (service = serviceProvider.GetService(field.FieldType)) is null)
                {
                    continue;
                }

                field.SetValue(createdObject, service);
            }

            return createdObject;
        }
    }
}
