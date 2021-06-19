namespace StoneAssemblies.MassAuth.Tests.Services
{
    using StoneAssemblies.MassAuth.Bank.Messages;
    using StoneAssemblies.MassAuth.Messages;
    using StoneAssemblies.MassAuth.Services;

    using Xunit;

    public class AuthorizationRequestMessageFactoryFacts
    {
        public class The_From_Method
        {
            [Fact]
            public void Initializes_A_Generic_AuthorizationRequestMessage_From_A_Regular_Message()
            {
                var authorizationRequestMessage =
                    AuthorizationRequestMessageFactory.From(new AccountBalanceRequestMessage());
                Assert.IsType<AuthorizationRequestMessage<AccountBalanceRequestMessage>>(authorizationRequestMessage);
            }

            [Fact]
            public void Sets_The_Payload()
            {
                var authorizationRequestMessage =
                    AuthorizationRequestMessageFactory.From(new AccountBalanceRequestMessage()) as
                        AuthorizationRequestMessage<AccountBalanceRequestMessage>;
                Assert.NotNull(authorizationRequestMessage?.Payload);
            }

            [Fact]
            public void Sets_The_Payload_With_The_Given_Message_Reference()
            {
                var accountBalanceRequestMessage = new AccountBalanceRequestMessage();
                var authorizationRequestMessage =
                    AuthorizationRequestMessageFactory.From(accountBalanceRequestMessage) as
                        AuthorizationRequestMessage<AccountBalanceRequestMessage>;
                Assert.Equal(accountBalanceRequestMessage, authorizationRequestMessage?.Payload);
            }
        }
    }
}