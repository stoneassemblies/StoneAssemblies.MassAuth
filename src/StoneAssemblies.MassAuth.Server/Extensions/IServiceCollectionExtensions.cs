namespace StoneAssemblies.MassAuth.Engine.Extensions
{
    using System;
    using System.Collections.Generic;
    using System.Reflection;

    using Microsoft.Extensions.DependencyInjection;

    using Serilog;

    using StoneAssemblies.MassAuth.Hosting.Extensions;
    using StoneAssemblies.MassAuth.Hosting.Services;
    using StoneAssemblies.MassAuth.Hosting.Services.Interfaces;
    using StoneAssemblies.MassAuth.Rules.Interfaces;

    public static class IServiceCollectionExtensions
    {
        private static readonly HashSet<Type> DiscoveredMessagesTypes = new HashSet<Type>();

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

        public static HashSet<Type> GetDiscoveredMessageTypes(this IServiceCollection serviceCollection)
        {
            return DiscoveredMessagesTypes;
        }

        private static IEnumerable<Type> AddRulesFromAssembly(
            this IServiceCollection serviceCollection,
            Assembly assembly)
        {
            foreach (var type in assembly.GetTypes())
            {
                var interfaces = type.GetInterfaces();
                if (!type.IsAbstract && interfaces.Length > 0)
                {
                    foreach (var ruleInterfaceType in interfaces)
                    {
                        if (ruleInterfaceType.FullName != null
                            && ruleInterfaceType.FullName.StartsWith(typeof(IRule<>).FullName))
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