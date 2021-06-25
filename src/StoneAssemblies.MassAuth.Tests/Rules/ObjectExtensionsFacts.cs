// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ObjectExtensionsFacts.cs" company="Stone Assemblies">
// Copyright © 2021 - 2021 Stone Assemblies. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace StoneAssemblies.MassAuth.Tests.Rules
{
    using StoneAssemblies.MassAuth.Bank.Messages;
    using StoneAssemblies.MassAuth.Rules.Extensions;

    using Xunit;

    /// <summary>
    ///     The object extensions facts.
    /// </summary>
    public class ObjectExtensionsFacts
    {
        /// <summary>
        ///     The get property value generic method.
        /// </summary>
        public class The_GetPropertyValue_Generic_Method
        {
            /// <summary>
            ///     Returns the value of an object property via reflection.
            /// </summary>
            [Fact]
            public void Return_The_Value_Of_An_Object_Property_Via_Reflection()
            {
                var accountBalanceRequestMessage = new AccountBalanceRequestMessage();
                var expectedValue = accountBalanceRequestMessage.PrimaryAccountNumber = "123456789";

                var value = accountBalanceRequestMessage.GetPropertyValue<string>(
                    nameof(AccountBalanceRequestMessage.PrimaryAccountNumber));
                Assert.Equal(expectedValue, value);
            }
        }

        /// <summary>
        ///     The get property value  method.
        /// </summary>
        public class The_GetPropertyValue_Method
        {
            /// <summary>
            ///     Returns the value of an object property via reflection.
            /// </summary>
            [Fact]
            public void Return_The_Value_Of_An_Object_Property_Via_Reflection()
            {
                var accountBalanceRequestMessage = new AccountBalanceRequestMessage();
                var expectedValue = accountBalanceRequestMessage.PrimaryAccountNumber = "123456789";

                var value = accountBalanceRequestMessage.GetPropertyValue(nameof(AccountBalanceRequestMessage.PrimaryAccountNumber));
                Assert.Equal(expectedValue, value);
            }
        }
    }
}