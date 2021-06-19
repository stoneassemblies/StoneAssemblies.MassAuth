namespace StoneAssemblies.MassAuth.Tests.Messages.TypeExtensions
{
    using StoneAssemblies.MassAuth.Bank.Messages;
    using StoneAssemblies.MassAuth.Messages;
    using StoneAssemblies.MassAuth.Messages.Extensions;

    using Xunit;

    public class TypeExtensionsFacts
    {
        public class The_GetFlatName_Method
        {
            [Fact]
            public void Returns_The_Expected_Name()
            {
                var flatName = typeof(AuthorizationRequestMessage<AccountBalanceRequestMessage>).GetFlatName();
                Assert.Equal("AuthorizationRequestMessage-AccountBalanceRequestMessage", flatName);
            }
        }
    }
}