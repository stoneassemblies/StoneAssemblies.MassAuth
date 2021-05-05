// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IRulesContainer.cs" company="Stone Assemblies">
// Copyright © 2021 - 2021 Stone Assemblies. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace StoneAssemblies.MassAuth.Hosting.Services.Interfaces
{
    using System.Collections.ObjectModel;

    using StoneAssemblies.MassAuth.Rules.Interfaces;

    /// <summary>
    ///     The RulesContainer interface.
    /// </summary>
    /// <typeparam name="TMessage">
    ///     The message type.
    /// </typeparam>
    public interface IRulesContainer<TMessage>
    {
        /// <summary>
        ///     Gets the rules.
        /// </summary>
        ReadOnlyCollection<IRule<TMessage>> Rules { get; }
    }
}