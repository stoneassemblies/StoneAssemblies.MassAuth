// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ServiceCollectionBusConfiguratorExtensions.cs" company="Stone Assemblies">
// Copyright © 2021 - 2021 Stone Assemblies. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace StoneAssemblies.MassAuth.Server.Extensions
{
    using System;
    using System.Collections.Generic;

    using MassTransit.ExtensionsDependencyInjectionIntegration;

    using Serilog;

    using StoneAssemblies.MassAuth.Server.Services;

    /// <summary>
    /// The service collection bus configurator extensions.
    /// </summary>
    public static class ServiceCollectionBusConfiguratorExtensions
    {
        /// <summary>
        /// The message type consumer type mappings.
        /// </summary>
        private static readonly Dictionary<Type, Type> MessageTypeConsumerTypeMappings = new Dictionary<Type, Type>();

        /// <summary>
        /// Adds authorization request consumers.
        /// </summary>
        /// <param name="configurator">
        /// The configurator.
        /// </param>
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

        /// <summary>
        /// Configure authorization request consumers.
        /// </summary>
        /// <param name="configurator">
        /// The configurator.
        /// </param>
        /// <param name="action">
        /// The action.
        /// </param>
        public static void ConfigureAuthorizationRequestConsumers(this IServiceCollectionBusConfigurator configurator, Action<Type, Type> action)
        {
            foreach (var messageTypeConsumerTypeMapping in MessageTypeConsumerTypeMappings)
            {
                action(messageTypeConsumerTypeMapping.Key, messageTypeConsumerTypeMapping.Value);
            }
        }

        /// <summary>
        /// Gets discovered message types.
        /// </summary>
        /// <param name="configurator">
        /// The configurator.
        /// </param>
        /// <returns>
        /// The <see cref="HashSet{Type}"/>.
        /// </returns>
        private static HashSet<Type> GetDiscoveredMessageTypes(this IServiceCollectionBusConfigurator configurator)
        {
            return configurator.Collection.GetDiscoveredMessageTypes();
        }
    }
}