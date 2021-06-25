// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AuthorizeByRuleAttributeFacts.cs" company="Stone Assemblies">
// Copyright © 2021 - 2021 Stone Assemblies. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace StoneAssemblies.MassAuth.Tests.Services.Attributes
{
    using StoneAssemblies.MassAuth.Services.Attributes;

    using Xunit;

    /// <summary>
    /// The authorize by rule attribute facts.
    /// </summary>
    public class AuthorizeByRuleAttributeFacts
    {
        /// <summary>
        /// The constructor.
        /// </summary>
        public class The_Constructor
        {
            /// <summary>
            /// Succeeds.
            /// </summary>
            [Fact]
            public void Succeeds()
            {
                var authorizeByRuleAttribute = new AuthorizeByRuleAttribute();
                Assert.NotNull(authorizeByRuleAttribute);
            }
        }
    }
}