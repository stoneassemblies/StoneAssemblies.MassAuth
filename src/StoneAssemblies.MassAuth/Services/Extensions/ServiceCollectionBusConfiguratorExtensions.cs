// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ServiceCollectionBusConfiguratorExtensions.cs" company="Stone Assemblies">
// Copyright © 2021 - 2021 Stone Assemblies. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace StoneAssemblies.MassAuth.Services.Extensions
{
    using MassTransit;

    using StoneAssemblies.Contrib.MassTransit.Services.Extensions;
    using StoneAssemblies.MassAuth.Messages;

    /// <summary>
    ///     The service collection bus configurator extensions.
    /// </summary>
    public static class ServiceCollectionBusConfiguratorExtensions
    {
        /// <summary>
        ///     Adds an authorization request client for a message.
        /// </summary>
        /// <param name="this">
        ///     The instance.
        /// </param>
        /// <param name="timeout">
        ///     The timeout.
        /// </param>
        /// <typeparam name="TMessage">
        ///     The message type
        /// </typeparam>
        public static void AddDefaultAuthorizationRequestClient<TMessage>(
            this IBusRegistrationConfigurator @this, RequestTimeout timeout = default)
            where TMessage : MessageBase
        {
            @this.AddDefaultRequestClient<AuthorizationRequestMessage<TMessage>>(timeout);
        }
    }
}