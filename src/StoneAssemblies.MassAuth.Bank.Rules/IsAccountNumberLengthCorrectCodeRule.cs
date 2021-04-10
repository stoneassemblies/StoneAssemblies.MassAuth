namespace StoneAssemblies.MassAuth.Bank.Rules
{
    using System;
    using System.Linq.Expressions;
    using System.Threading.Tasks;

    using Microsoft.CodeAnalysis.CSharp.Scripting;
    using Microsoft.CodeAnalysis.Scripting;

    using Serilog;

    using StoneAssemblies.MassAuth.Messages;
    using StoneAssemblies.MassAuth.Rules.Extensions;
    using StoneAssemblies.MassAuth.Rules.Interfaces;

    /// <summary>
    ///     A csharp script code rule.
    /// </summary>
    public class IsAccountNumberLengthCorrectCodeRule : IRule<AuthorizationRequestMessage<AccountBalanceRequestMessage>>
    {
        /// <summary>
        ///     The sample code.
        /// </summary>
        private const string Code =
            "m => m.GetPropertyValue(\"Payload\").GetPropertyValue<string>(\"PrimaryAccountNumber\").Length == 5";

        /// <summary>
        ///     The delegate of the rule.
        /// </summary>
        private readonly Func<object, bool> delegateRule;

        /// <summary>
        ///     Initializes a new instance of the <see cref="IsAccountNumberLengthCorrectCodeRule" /> class.
        /// </summary>
        public IsAccountNumberLengthCorrectCodeRule()
        {
            var options = ScriptOptions.Default
                .AddReferences(typeof(Expression).Assembly, typeof(ObjectExtensions).Assembly).AddImports(
                    "System",
                    "System.Linq",
                    typeof(ObjectExtensions).Namespace);

            try
            {
                var script = CSharpScript.Create<Func<object, bool>>(Code, options);
                this.delegateRule = script.CreateDelegate().Invoke().Result;
            }
            catch (Exception e)
            {
                Log.Error(e, "Error compiling the rule '{Rule}' code '{Code}'", this.Name, Code);
            }
        }

        /// <summary>
        ///     Gets a value indicating whether is enabled.
        /// </summary>
        public bool IsEnabled => this.delegateRule != null;

        /// <summary>
        ///     The name.
        /// </summary>
        public string Name => $"Is Account Number Length Correct Code Rule '{Code}'";

        /// <summary>
        ///     Gets the priority.
        /// </summary>
        public int Priority { get; }

        /// <summary>
        ///     Evaluates the rule.
        /// </summary>
        /// <param name="message">
        ///     The message.
        /// </param>
        /// <returns>
        ///     The <see cref="Task" />.
        /// </returns>
        public Task<bool> EvaluateAsync(AuthorizationRequestMessage<AccountBalanceRequestMessage> message)
        {
            var delegateResult = true;
            try
            {
                if (this.delegateRule != null)
                {
                    delegateResult = this.delegateRule.Invoke(message);
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error evaluating rules");
            }

            return Task.FromResult(delegateResult);
        }
    }
}