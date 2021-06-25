// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RulesContainer.cs" company="Stone Assemblies">
// Copyright © 2021 - 2021 Stone Assemblies. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace StoneAssemblies.MassAuth.Hosting.Services
{
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;

    using StoneAssemblies.MassAuth.Hosting.Services.Interfaces;
    using StoneAssemblies.MassAuth.Rules.Interfaces;

    /// <summary>
    ///     The rules container.
    /// </summary>
    /// <typeparam name="TMessage">
    ///     The type of message.
    /// </typeparam>
    public class RulesContainer<TMessage> : IRulesContainer<TMessage>
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="RulesContainer{TMessage}" /> class.
        /// </summary>
        /// <param name="rules">
        ///     The rules.
        /// </param>
        public RulesContainer(IEnumerable<IRule<TMessage>> rules)
        {
            this.Rules = rules.Where(rule => rule.IsEnabled).OrderBy(rule => rule.Priority).ToList().AsReadOnly();
        }

        /// <summary>
        ///     Gets the rules.
        /// </summary>
        public ReadOnlyCollection<IRule<TMessage>> Rules { get; }
    }
}
