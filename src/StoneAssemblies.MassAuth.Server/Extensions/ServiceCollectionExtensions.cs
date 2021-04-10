// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ServiceCollectionExtensions.cs" company="Stone Assemblies">
// Copyright © 2021 - 2021 Stone Assemblies. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace StoneAssemblies.MassAuth.Server.Extensions
{
    using System;
    using System.Collections.Generic;
    using System.Reflection;

    using Microsoft.Extensions.DependencyInjection;

    using Serilog;

    using StoneAssemblies.MassAuth.Hosting.Extensions;
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
        public static void AddAutoDiscoveredRules(this IServiceCollection serviceCollection)
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

        /// <summary>
        ///     Gets discovered message types.
        /// </summary>
        /// <param name="serviceCollection">
        ///     The service collection.
        /// </param>
        /// <returns>
        ///     The <see cref="HashSet{Type}" />.
        /// </returns>
        public static HashSet<Type> GetDiscoveredMessageTypes(this IServiceCollection serviceCollection)
        {
            return DiscoveredMessagesTypes;
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
                            && !string.IsNullOrWhiteSpace(fullName) && ruleInterfaceType.FullName.StartsWith(fullName))
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
    }
}