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
        /// The sample code.
        /// </summary>
        private const string Code = "m => m.GetPropertyValue(\"Payload\").GetPropertyValue(\"PrimaryAccountNumber\").Length == 5";

        /// <summary>
        ///     The delegate of the rule.
        /// </summary>
        private readonly Func<object, bool> delegateRule;

        public IsAccountNumberLengthCorrectCodeRule()
        {
            var options = ScriptOptions.Default
                .AddReferences(typeof(Expression).Assembly, typeof(ObjectExtensions).Assembly).AddImports(
                    "System.Linq",
                    typeof(ObjectExtensions).Namespace);

            var script = CSharpScript.Create<Func<object, bool>>(Code, options);

            this.delegateRule = script.CreateDelegate().Invoke().Result;
        }

        public bool IsEnabled => true;

        public string Name => $"Is Account Number Length Correct Code Rule '{Code}'";

        public int Priority { get; }

        public Task<bool> EvaluateAsync(AuthorizationRequestMessage<AccountBalanceRequestMessage> message)
        {
            try
            {
                var invoke = this.delegateRule.Invoke(message);
                return Task.FromResult(invoke);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error evaluating rules");
            }

            return Task.FromResult(false);
        }
    }
}