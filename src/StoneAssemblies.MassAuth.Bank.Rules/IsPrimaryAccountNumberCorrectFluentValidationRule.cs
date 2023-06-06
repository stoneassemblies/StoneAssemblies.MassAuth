// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IsPrimaryAccountNumberCorrectFluentValidationRule.cs" company="Stone Assemblies">
// Copyright © 2021 - 2021 Stone Assemblies. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace StoneAssemblies.MassAuth.Bank.Rules
{
    using System.Collections.Generic;
    using System.Threading.Tasks;

    using FluentValidation;

    using StoneAssemblies.MassAuth.Bank.Messages;
    using StoneAssemblies.MassAuth.Messages;
    using StoneAssemblies.MassAuth.Rules.Interfaces;

    /// <summary>
    /// The AccountBalanceRequestMessageFluentValidationRule class
    /// </summary>
    public class IsPrimaryAccountNumberCorrectFluentValidationRule : AbstractValidator<AuthorizationRequestMessage<AccountBalanceRequestMessage>>, IRule<AuthorizationRequestMessage<AccountBalanceRequestMessage>>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="IsPrimaryAccountNumberCorrectFluentValidationRule"/> class.
        /// </summary>
        public IsPrimaryAccountNumberCorrectFluentValidationRule()
        {
            this.RuleFor(message => message.Payload.PrimaryAccountNumber)
                .Matches("^\\d+$")
                .WithErrorCode("42")
                .NotNull()
                .NotEmpty();
        }

        /// <summary>
        ///     Gets a value indicating whether the rule is enabled.
        /// </summary>
        public bool IsEnabled { get; } = true;

        /// <summary>
        ///     Gets the name.
        /// </summary>
        public string Name { get; } = "Is Primary Account Number Format Correct";

        /// <summary>
        ///     Gets the priority.
        /// </summary>
        public int Priority { get; } = 1;

        /// <summary>
        /// The evaluate async.
        /// </summary>
        /// <param name="message">
        /// The message.
        /// </param>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        public async Task<EvaluationResult> EvaluateAsync(AuthorizationRequestMessage<AccountBalanceRequestMessage> message)
        {
            var validationResult = await this.ValidateAsync(message);
            if (!validationResult.IsValid)
            {
                var data = new Dictionary<string, object>
                           {
                               ["ErrorCode"] = validationResult.Errors[0].ErrorCode, 
                               ["Description"] = validationResult.Errors[0].ErrorMessage
                           };

                return EvaluationResult.Error(data);
            }

            return EvaluationResult.Success();
        }
    }
}