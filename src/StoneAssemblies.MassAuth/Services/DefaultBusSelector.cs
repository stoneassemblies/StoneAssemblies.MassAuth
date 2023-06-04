// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DefaultBusSelector.cs" company="Stone Assemblies">
// Copyright © 2021 - 2021 Stone Assemblies. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace StoneAssemblies.MassAuth.Services
{
    using System;
    using System.Collections.Generic;

    using MassTransit;

    using StoneAssemblies.MassAuth.Services.Interfaces;

    /// <summary>
    ///     The default bus selector.
    /// </summary>
    /// <typeparam name="TMessage">
    ///     The message type.
    /// </typeparam>
    public class DefaultBusSelector<TMessage> : IBusSelector<TMessage>
        where TMessage : class
    {
        /// <summary>
        ///     The bus.
        /// </summary>
        private readonly IBus bus;

        /// <summary>
        ///     Initializes a new instance of the <see cref="DefaultBusSelector{TMessage}" /> class.
        /// </summary>
        /// <param name="bus">
        ///     The bus.
        /// </param>
        public DefaultBusSelector(IBus bus)
        {
            this.bus = bus ?? throw new ArgumentNullException(nameof(bus));
        }

        /// <summary>
        ///     The select client factories.
        /// </summary>
        /// <param name="message">
        ///     The message.
        /// </param>
        /// <returns>
        ///     The <see cref="IAsyncEnumerable{IClientFactory}" />.
        /// </returns>
#pragma warning disable 1998
        public async IAsyncEnumerable<IClientFactory> SelectClientFactories(TMessage message)
#pragma warning restore 1998
        {
            yield return this.bus.CreateClientFactory();
        }

        /// <summary>
        ///     The select client factories.
        /// </summary>
        /// <param name="message">
        ///     The message.
        /// </param>
        /// <returns>
        ///     The <see cref="IAsyncEnumerable{IClientFactory}" />.
        /// </returns>
#pragma warning disable 1998
        public async IAsyncEnumerable<IClientFactory> SelectClientFactories(object message)
#pragma warning restore 1998
        {
            yield return this.bus.CreateClientFactory();
        }
    }
}