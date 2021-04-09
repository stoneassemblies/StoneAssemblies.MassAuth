namespace StoneAssemblies.MassAuth.Engine.Extensions
{
    using System;
    using System.Collections.Generic;

    using MassTransit.ExtensionsDependencyInjectionIntegration;

    using Serilog;

    using StoneAssemblies.MassAuth.Engine.Services;

    public static class IServiceCollectionBusConfiguratorExtensions
    {
        private static readonly Dictionary<Type, Type> MessageTypeConsumerTypeMappings = new Dictionary<Type, Type>();

        public static void AddAuthorizationRequestConsumers(this IServiceCollectionBusConfigurator configurator)
        {
            foreach (var messagesType in configurator.GetDiscoveredMessageTypes())
            {
                var consumerType = typeof(AuthorizationRequestMessageConsumer<>).MakeGenericType(messagesType);
                Log.Information("Registering consumer {ConsumerType}", consumerType);
                configurator.AddConsumer(consumerType);
                MessageTypeConsumerTypeMappings[messagesType] = consumerType;
            }
        }

        public static void ConfigureAuthorizationRequestConsumers(this IServiceCollectionBusConfigurator configurator, Action<Type, Type> action)
        {
            foreach (var messageTypeConsumerTypeMapping in MessageTypeConsumerTypeMappings)
            {
                action(messageTypeConsumerTypeMapping.Key, messageTypeConsumerTypeMapping.Value);
            }
        }

        public static HashSet<Type> GetDiscoveredMessageTypes(this IServiceCollectionBusConfigurator configurator)
        {
            return configurator.Collection.GetDiscoveredMessageTypes();
        }
    }
}