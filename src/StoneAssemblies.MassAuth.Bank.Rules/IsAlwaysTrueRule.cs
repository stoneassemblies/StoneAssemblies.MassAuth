namespace StoneAssemblies.MassAuth.Bank.Rules
{
    using System.Threading.Tasks;

    using StoneAssemblies.MassAuth.Messages;
    using StoneAssemblies.MassAuth.Rules.Interfaces;

    public class IsAlwaysTrueRule : IRule<AuthorizationRequestMessage<AccountBalanceRequestMessage>>
    {
        public bool IsEnabled => true;

        public string Name => "Is Always True Rule";

        public int Priority { get; }

        public Task<bool> EvaluateAsync(AuthorizationRequestMessage<AccountBalanceRequestMessage> message)
        {
            return Task.FromResult(true);
        }
    }
}