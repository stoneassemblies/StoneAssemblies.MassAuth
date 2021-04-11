// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IsAlwaysTrueRule.cs" company="Stone Assemblies">
// Copyright © 2021 - 2021 Stone Assemblies. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace StoneAssemblies.MassAuth.Bank.Rules
{
    using System.Threading.Tasks;

    using StoneAssemblies.MassAuth.Bank.Messages;
    using StoneAssemblies.MassAuth.Messages;
    using StoneAssemblies.MassAuth.Rules.Interfaces;

    /// <summary>
    ///     The is always true rule.
    /// </summary>
    public class IsAlwaysTrueRule : IRule<AuthorizationRequestMessage<AccountBalanceRequestMessage>>
    {
        /// <summary>
        ///     Gets a value indicating whether the rule is enabled.
        /// </summary>
        public bool IsEnabled => true;

        /// <summary>
        ///     Gets the name.
        /// </summary>
        public string Name => "Is Always True Rule";

        /// <summary>
        ///     Gets the priority.
        /// </summary>
        public int Priority { get; } = 0;

        /// <summary>
        ///     Evaluate the rule asynchronously.
        /// </summary>
        /// <param name="message">
        ///     The message.
        /// </param>
        /// <returns>
        ///     The <see cref="Task" />.
        /// </returns>
        public Task<bool> EvaluateAsync(AuthorizationRequestMessage<AccountBalanceRequestMessage> message)
        {
            return Task.FromResult(true);
        }
    }
}