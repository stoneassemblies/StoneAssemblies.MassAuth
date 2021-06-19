// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AuthorizationRequestMessageFactoryFacts.cs" company="Stone Assemblies">
// Copyright © 2021 - 2021 Stone Assemblies. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace StoneAssemblies.MassAuth.Tests.Services
{
    using StoneAssemblies.MassAuth.Bank.Messages;
    using StoneAssemblies.MassAuth.Messages;
    using StoneAssemblies.MassAuth.Services;

    using Xunit;

    /// <summary>
    /// The authorization request message factory facts.
    /// </summary>
    public class AuthorizationRequestMessageFactoryFacts
    {
        /// <summary>
        /// The from method.
        /// </summary>
        public class The_From_Method
        {
            /// <summary>
            /// Initializes a generic authorization request message from a regular message.
            /// </summary>
            [Fact]
            public void Initializes_A_Generic_AuthorizationRequestMessage_From_A_Regular_Message()
            {
                var authorizationRequestMessage =
                    AuthorizationRequestMessageFactory.From(new AccountBalanceRequestMessage());
                Assert.IsType<AuthorizationRequestMessage<AccountBalanceRequestMessage>>(authorizationRequestMessage);
            }

            /// <summary>
            /// Sets the payload.
            /// </summary>
            [Fact]
            public void Sets_The_Payload()
            {
                var authorizationRequestMessage =
                    AuthorizationRequestMessageFactory.From(new AccountBalanceRequestMessage()) as
                        AuthorizationRequestMessage<AccountBalanceRequestMessage>;
                Assert.NotNull(authorizationRequestMessage?.Payload);
            }

            /// <summary>
            /// Sets the payload with the given message reference.
            /// </summary>
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