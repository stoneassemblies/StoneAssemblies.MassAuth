// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PredicateBasedBusSelector.cs" company="Stone Assemblies">
// Copyright © 2021 - 2021 Stone Assemblies. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace StoneAssemblies.MassAuth.Services
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using MassTransit;

    using StoneAssemblies.MassAuth.Services.Interfaces;

    /// <summary>
    ///     The predicate based bus selector.
    /// </summary>
    /// <typeparam name="TMessage">
    ///     The message type.
    /// </typeparam>
    public class PredicateBasedBusSelector<TMessage> : IBusSelector<TMessage>
        where TMessage : class
    {
        /// <summary>
        ///     The buses.
        /// </summary>
        private readonly List<IBus> buses;

        /// <summary>
        ///     The predicate.
        /// </summary>
        private readonly IBusSelectorPredicate<TMessage> predicate;

        /// <summary>
        ///     Initializes a new instance of the <see cref="PredicateBasedBusSelector{TMessage}" /> class.
        /// </summary>
        /// <param name="buses">
        ///     The buses.
        /// </param>
        /// <param name="predicate">
        ///     The predicate.
        /// </param>
        public PredicateBasedBusSelector(IEnumerable<IBus> buses, IBusSelectorPredicate<TMessage> predicate)
        {
            if (buses == null)
            {
                throw new ArgumentNullException(nameof(buses));
            }

            this.buses = buses.ToList();
            this.predicate = predicate ?? throw new ArgumentNullException(nameof(predicate));
        }

        /// <summary>
        ///     Selects the list of client factories.
        /// </summary>
        /// <param name="message">
        ///     The message.
        /// </param>
        /// <returns>
        ///     The client factories.
        /// </returns>
        public async IAsyncEnumerable<IClientFactory> SelectClientFactories(TMessage message)
        {
            foreach (var bus in this.buses)
            {
                if (await this.predicate.IsMatch(bus, message))
                {
                    yield return bus.CreateClientFactory();
                }
            }
        }

        /// <summary>
        ///     Selects the list of client factories.
        /// </summary>
        /// <param name="message">
        ///     The message.
        /// </param>
        /// <returns>
        ///     The client factories.
        /// </returns>
        public IAsyncEnumerable<IClientFactory> SelectClientFactories(object message)
        {
            return this.SelectClientFactories(message as TMessage);
        }
    }
}