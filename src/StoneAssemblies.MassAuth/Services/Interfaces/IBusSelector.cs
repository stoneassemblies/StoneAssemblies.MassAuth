// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IBusSelector.cs" company="Stone Assemblies">
// Copyright © 2021 - 2021 Stone Assemblies. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace StoneAssemblies.MassAuth.Services.Interfaces
{
    using System.Collections.Generic;

    using MassTransit;

    /// <summary>
    ///     The BusSelector interface.
    /// </summary>
    public interface IBusSelector
    {
        /// <summary>
        ///     The select client factories.
        /// </summary>
        /// <param name="message">
        ///     The message.
        /// </param>
        /// <returns>
        ///     The <see cref="IAsyncEnumerable{IClientFactory}" />.
        /// </returns>
        IAsyncEnumerable<IClientFactory> SelectClientFactories(object message);
    }

    /// <summary>
    ///     The BusSelector interface.
    /// </summary>
    /// <typeparam name="TMessage">
    ///     The message type.
    /// </typeparam>
    public interface IBusSelector<in TMessage> : IBusSelector
        where TMessage : class
    {
        /// <summary>
        ///     The select client factories.
        /// </summary>
        /// <param name="message">
        ///     The message.
        /// </param>
        /// <returns>
        ///     The <see cref="IAsyncEnumerable{IClientFactory}" />.
        /// </returns>
        IAsyncEnumerable<IClientFactory> SelectClientFactories(TMessage message);
    }
}