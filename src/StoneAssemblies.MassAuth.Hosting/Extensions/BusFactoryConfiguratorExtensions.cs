// --------------------------------------------------------------------------------------------------------------------
// <copyright file="BusFactoryConfiguratorExtensions.cs" company="Stone Assemblies">
// Copyright © 2021 - 2021 Stone Assemblies. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace StoneAssemblies.MassAuth.Hosting.Extensions
{
    using System;

    using MassTransit;

    using StoneAssemblies.MassAuth.Messages.Extensions;

    // using StoneAssemblies.Contrib.MassTransit.Extensions;

    /// <summary>
    ///     The bus factory configurator extensions.
    /// </summary>
    public static class BusFactoryConfiguratorExtensions
    {
        /// <summary>
        ///     Adds a receive endpoint with a queue name based on message type.
        /// </summary>
        /// <param name="busFactoryConfigurator">
        ///     The bus factory configurator.
        /// </param>
        /// <param name="configureEndpoint">
        ///     The configure endpoint.
        /// </param>
        /// <typeparam name="TMessage">
        ///     The type.
        /// </typeparam>
        public static void DefaultReceiveEndpoint<TMessage>(
            this IBusFactoryConfigurator busFactoryConfigurator, Action<IReceiveEndpointConfigurator> configureEndpoint)
        {
            busFactoryConfigurator.DefaultReceiveEndpoint(typeof(TMessage), configureEndpoint);
        }

        /// <summary>
        ///     Adds a receive endpoint with a queue name based on message type.
        /// </summary>
        /// <param name="busFactoryConfigurator">
        ///     The bus factory configurator.
        /// </param>
        /// <param name="messageType">
        ///     The message type.
        /// </param>
        /// <param name="configureEndpoint">
        ///     The configure endpoint.
        /// </param>
        public static void DefaultReceiveEndpoint(
            this IBusFactoryConfigurator busFactoryConfigurator,
            Type messageType,
            Action<IReceiveEndpointConfigurator> configureEndpoint)
        {
            busFactoryConfigurator.ReceiveEndpoint(messageType.GetFlatName(), configureEndpoint);
        }
    }
}