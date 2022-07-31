namespace StoneAssemblies.MassAuth.Bank.Rules;

using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using FluentValidation;

using StoneAssemblies.MassAuth.Bank.Messages;
using StoneAssemblies.MassAuth.Messages;
using StoneAssemblies.MassAuth.Rules.Interfaces;

/// <summary>
/// The AccountBalanceRequestMessageFluentValidationRule class
/// </summary>
public class AccountBalanceRequestMessageFluentValidationRule : AbstractValidator<AuthorizationRequestMessage<AccountBalanceRequestMessage>>, IRule<AuthorizationRequestMessage<AccountBalanceRequestMessage>>
{
    /// <summary>
    /// 
    /// </summary>
    public bool IsEnabled { get; } = true;

    /// <summary>
    /// 
    /// </summary>
    public string Name { get; } = "Fluent validation rule for AccountBalanceRequestMessage";

    /// <summary>
    /// 
    /// </summary>
    public int Priority { get; } = 1;

    /// <summary>
    /// 
    /// </summary>
    public AccountBalanceRequestMessageFluentValidationRule()
    {
        this.RuleFor(message => message.Payload.PrimaryAccountNumber)
            .Matches("^\\d+$")
            .NotNull()
            .NotEmpty();
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="message"></param>
    /// <returns></returns>
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