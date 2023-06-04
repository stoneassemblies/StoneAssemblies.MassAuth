// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IBusSelectorPredicate.cs" company="Stone Assemblies">
// Copyright © 2021 - 2021 Stone Assemblies. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace StoneAssemblies.MassAuth.Services.Interfaces
{
    using System.Threading.Tasks;

    using MassTransit;

    /// <summary>
    /// The BusSelectorPredicate interface.
    /// </summary>
    /// <typeparam name="TMessage">
    /// The message type.
    /// </typeparam>
    public interface IBusSelectorPredicate<in TMessage>
    {
        /// <summary>
        /// The is match.
        /// </summary>
        /// <param name="bus">
        /// The bus.
        /// </param>
        /// <param name="message">
        /// The message.
        /// </param>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        Task<bool> IsMatch(IBus bus, TMessage message);
    }
}