// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IsThisAccountMineRule.cs" company="Stone Assemblies">
// Copyright © 2021 - 2021 Stone Assemblies. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace StoneAssemblies.MassAuth.Bank.Rules
{
    using System.Collections.Generic;
    using System.Threading.Tasks;

    using StoneAssemblies.MassAuth.Bank.Messages;
    using StoneAssemblies.MassAuth.Messages;
    using StoneAssemblies.MassAuth.Rules.Interfaces;

    /// <summary>
    ///     The is account owner rule.
    /// </summary>
    public class IsAccountOwnerRule : IRule<AuthorizationRequestMessage<AccountBalanceRequestMessage>>
    {
        /// <summary>
        ///     Gets a value indicating whether the rule is enabled.
        /// </summary>
        public bool IsEnabled => true;

        /// <summary>
        ///     Gets the name.
        /// </summary>
        public string Name => "Is Account Owner";

        /// <summary>
        ///     Gets the priority.
        /// </summary>
        public int Priority { get; } = 5;

        /// <summary>
        ///     Evaluate the rule asynchronously.
        /// </summary>
        /// <param name="message">
        ///     The message.
        /// </param>
        /// <returns>
        ///     The <see cref="Task" />.
        /// </returns>
        public Task<EvaluationResult> EvaluateAsync(AuthorizationRequestMessage<AccountBalanceRequestMessage> message)
        {
            var accounts = new List<string> { "12345", "54321" };
            var error = new Dictionary<string, object>
                        {
                            ["ErrorCode"] = 23,
                            ["Description"] = "You are not the account owner"
                        };

            if (message.UserId == "alexfdezsauco" && !accounts.Contains(message.Payload.PrimaryAccountNumber))
            {
                return Task.FromResult(EvaluationResult.Error(error));
            }

            if (accounts.Contains(message.Payload.PrimaryAccountNumber))
            {
                return Task.FromResult(EvaluationResult.Error(error));
            }

            return Task.FromResult(EvaluationResult.Success());
        }
    }
}