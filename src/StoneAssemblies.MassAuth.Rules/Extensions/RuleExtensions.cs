// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RuleExtensions.cs" company="Stone Assemblies">
// Copyright © 2021 - 2021 Stone Assemblies. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace StoneAssemblies.MassAuth.Rules.Extensions
{
    using System.Threading.Tasks;

    using Serilog;

    using StoneAssemblies.MassAuth.Rules.Interfaces;

    /// <summary>
    ///     The rule extensions.
    /// </summary>
    public static class RuleExtensions
    {
        /// <summary>
        ///     The fail async.
        /// </summary>
        /// <param name="rule">
        ///     The rule.
        /// </param>
        /// <param name="message">
        ///     The message.
        /// </param>
        /// <typeparam name="TMessage">
        ///     The message type.
        /// </typeparam>
        /// <returns>
        ///     The <see cref="Task" />.
        /// </returns>
        public static async Task<bool> FailAsync<TMessage>(this IRule<TMessage> rule, TMessage message)
        {
            Log.Information("Evaluating rule '{RuleName}'", rule.Name);

            return !await rule.EvaluateAsync(message);
        }

        /// <summary>
        ///     The pass async.
        /// </summary>
        /// <param name="rule">
        ///     The rule.
        /// </param>
        /// <param name="message">
        ///     The message.
        /// </param>
        /// <typeparam name="TMessage">
        ///     The message type.
        /// </typeparam>
        /// <returns>
        ///     The <see cref="Task" />.
        /// </returns>
        public static async Task<bool> PassAsync<TMessage>(this IRule<TMessage> rule, TMessage message)
        {
            Log.Information("Evaluating rule '{RuleName}'", rule.Name);

            return await rule.EvaluateAsync(message);
        }
    }
}
