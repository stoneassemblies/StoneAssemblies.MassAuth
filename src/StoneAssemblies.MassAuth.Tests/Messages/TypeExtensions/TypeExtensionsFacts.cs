// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TypeExtensionsFacts.cs" company="Stone Assemblies">
// Copyright © 2021 - 2021 Stone Assemblies. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace StoneAssemblies.MassAuth.Tests.Messages.TypeExtensions
{
    using StoneAssemblies.Contrib.MassTransit.Extensions;
    using StoneAssemblies.MassAuth.Bank.Messages;
    using StoneAssemblies.MassAuth.Messages;

    using Xunit;

    /// <summary>
    ///     The type extensions facts.
    /// </summary>
    public class TypeExtensionsFacts
    {
        /// <summary>
        ///     The get flat name method.
        /// </summary>
        public class The_GetFlatName_Method
        {
            /// <summary>
            ///     Returns the expected name.
            /// </summary>
            [Fact]
            public void Returns_The_Expected_Name()
            {
                var flatName = typeof(AuthorizationRequestMessage<AccountBalanceRequestMessage>).GetFlatName();
                Assert.Equal("AuthorizationRequestMessage-AccountBalanceRequestMessage", flatName);
            }
        }
    }
}
