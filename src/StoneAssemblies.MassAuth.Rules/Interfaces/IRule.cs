// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IRule.cs" company="Stone Assemblies">
// Copyright © 2021 - 2021 Stone Assemblies. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace StoneAssemblies.MassAuth.Rules.Interfaces
{
    using System.Threading.Tasks;

    /// <summary>
    ///     The Rule interface.
    /// </summary>
    /// <typeparam name="TMessage">
    ///     The message type.
    /// </typeparam>
    public interface IRule<TMessage>
    {
        /// <summary>
        ///     Gets a value indicating whether is enabled.
        /// </summary>
        bool IsEnabled { get; }

        /// <summary>
        ///     Gets the name.
        /// </summary>
        string Name { get; }

        /// <summary>
        ///     Gets the priority.
        /// </summary>
        int Priority { get; }

        /// <summary>
        ///     Evaluate the rule asynchronously.
        /// </summary>
        /// <param name="message">
        ///     The message.
        /// </param>
        /// <returns>
        ///     The <see cref="Task" />.
        /// </returns>
        Task<bool> EvaluateAsync(TMessage message);
    }
}