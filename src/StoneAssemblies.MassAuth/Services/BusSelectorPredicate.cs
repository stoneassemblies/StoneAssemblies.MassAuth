// --------------------------------------------------------------------------------------------------------------------
// <copyright file="BusSelectorPredicate.cs" company="Stone Assemblies">
// Copyright © 2021 - 2021 Stone Assemblies. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace StoneAssemblies.MassAuth.Services
{
    using System;
    using System.Threading.Tasks;

    using MassTransit;

    using StoneAssemblies.MassAuth.Services.Interfaces;

    /// <summary>
    ///     The bus selector predicate.
    /// </summary>
    /// <typeparam name="TMessage">
    ///     The message type.
    /// </typeparam>
    public class BusSelectorPredicate<TMessage> : IBusSelectorPredicate<TMessage>
    {
        /// <summary>
        ///     The predicate.
        /// </summary>
        private readonly Func<IBus, TMessage, Task<bool>> predicate;

        /// <summary>
        ///     Initializes a new instance of the <see cref="BusSelectorPredicate{TMessage}" /> class.
        /// </summary>
        /// <param name="predicate">
        ///     The predicate.
        /// </param>
        public BusSelectorPredicate(Func<IBus, TMessage, Task<bool>> predicate)
        {
            this.predicate = predicate ?? throw new ArgumentNullException(nameof(predicate));
        }

        /// <summary>
        ///     The is match.
        /// </summary>
        /// <param name="bus">
        ///     The bus.
        /// </param>
        /// <param name="message">
        ///     The message.
        /// </param>
        /// <returns>
        ///     The <see cref="Task" />.
        /// </returns>
        public async Task<bool> IsMatch(IBus bus, TMessage message)
        {
            return await this.predicate(bus, message);
        }
    }
}