// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ServiceCollectionExtensions.cs" company="Stone Assemblies">
// Copyright © 2021 - 2021 Stone Assemblies. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace StoneAssemblies.MassAuth.Hosting.Extensions
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;

    using Microsoft.Extensions.DependencyInjection;

    using Serilog;

    using StoneAssemblies.Extensibility.Services.Interfaces;
    using StoneAssemblies.Hosting.Extensions;
    using StoneAssemblies.MassAuth.Hosting.Services;
    using StoneAssemblies.MassAuth.Hosting.Services.Interfaces;
    using StoneAssemblies.MassAuth.Rules.Interfaces;

    /// <summary>
    ///     The service collection extensions.
    /// </summary>
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        ///     The discovered messages types.
        /// </summary>
        private static readonly HashSet<Type> DiscoveredMessagesTypes = new HashSet<Type>();

        /// <summary>
        ///     The add auto discovered rules.
        /// </summary>
        /// <param name="serviceCollection">
        ///     The service collection.
        /// </param>
        /// <param name="autoDiscover">
        ///     Auto discover rules and message types.
        /// </param>
        public static void AddRules(this IServiceCollection serviceCollection, bool autoDiscover = true)
        {
            serviceCollection.AddSingleton(typeof(IRulesContainer<>), typeof(RulesContainer<>));
            serviceCollection.AddMessagesTypesFromAlreadyRegisteredRules();
            if (autoDiscover)
            {
                serviceCollection.AutoDiscoverRulesAndMessageTypes();
            }
        }

        /// <summary>
        ///     Gets discovered message types.
        /// </summary>
        /// <param name="serviceCollection">
        ///     The service collection.
        /// </param>
        /// <returns>
        ///     The <see cref="HashSet{Type}" />.
        /// </returns>
#pragma warning disable IDE0060 // Remove unused parameter
        public static HashSet<Type> GetDiscoveredMessageTypes(this IServiceCollection serviceCollection)
#pragma warning restore IDE0060 // Remove unused parameter
        {
            return DiscoveredMessagesTypes;
        }

        /// <summary>
        ///     Add messages types from already registered rules.
        /// </summary>
        /// <param name="serviceCollection">
        ///     The service collection.
        /// </param>
        private static void AddMessagesTypesFromAlreadyRegisteredRules(this IServiceCollection serviceCollection)
        {
            var fullName = typeof(IRule<>).FullName;
            if (!string.IsNullOrWhiteSpace(fullName))
            {
                var registeredRulesTypes = serviceCollection
                    .Where(descriptor => !string.IsNullOrWhiteSpace(descriptor.ServiceType.FullName) && descriptor.ServiceType.FullName.StartsWith(fullName))
                    .Select(descriptor => descriptor.ServiceType);
                foreach (var registeredRulesType in registeredRulesTypes)
                {
                    var genericArguments = registeredRulesType.GetGenericArguments();
                    if (genericArguments.Length != 1)
                    {
                        continue;
                    }

                    var messageType = genericArguments[0];
                    if (!DiscoveredMessagesTypes.Contains(messageType))
                    {
                        DiscoveredMessagesTypes.Add(messageType);
                    }
                }
            }
        }

        /// <summary>
        ///     Add rules from assembly.
        /// </summary>
        /// <param name="serviceCollection">
        ///     The service collection.
        /// </param>
        /// <param name="assembly">
        ///     The assembly.
        /// </param>
        /// <returns>
        ///     The <see cref="IEnumerable{Type}" />.
        /// </returns>
        private static IEnumerable<Type> AddRulesFromAssembly(
            this IServiceCollection serviceCollection,
            Assembly assembly)
        {
            var fullName = typeof(IRule<>).FullName;

            foreach (var type in assembly.GetTypes())
            {
                var interfaces = type.GetInterfaces();
                if (!type.IsAbstract && interfaces.Length > 0)
                {
                    foreach (var ruleInterfaceType in interfaces)
                    {
                        if (!string.IsNullOrWhiteSpace(ruleInterfaceType.FullName)
                            && !string.IsNullOrWhiteSpace(fullName) 
                            && ruleInterfaceType.FullName.StartsWith(fullName))
                        {
                            // TODO: Improve this?
                            var genericArguments = ruleInterfaceType.GetGenericArguments();
                            if (genericArguments.Length != 1)
                            {
                                continue;
                            }

                            var messageType = genericArguments[0];

                            Log.Information("Registering rule {RuleType}", type);

                            serviceCollection.AddSingleton(ruleInterfaceType, type);

                            yield return messageType;
                        }
                    }
                }
            }
        }

        /// <summary>
        ///     Auto discover rules.
        /// </summary>
        /// <param name="serviceCollection">
        ///     The service collection.
        /// </param>
        private static void AutoDiscoverRulesAndMessageTypes(this IServiceCollection serviceCollection)
        {
            var extensionManager = serviceCollection.GetRegisteredInstance<IExtensionManager>();

            foreach (var assembly in extensionManager.GetExtensionAssemblies())
            {
                foreach (var messageType in serviceCollection.AddRulesFromAssembly(assembly))
                {
                    if (!DiscoveredMessagesTypes.Contains(messageType))
                    {
                        DiscoveredMessagesTypes.Add(messageType);
                    }
                }
            }
        }
    }
}