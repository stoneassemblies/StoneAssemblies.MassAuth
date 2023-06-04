// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DefaultBusSelectorFacts.cs" company="Stone Assemblies">
// Copyright © 2021 - 2021 Stone Assemblies. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace StoneAssemblies.MassAuth.Tests.Services
{
    using System;
    using System.Threading.Tasks;

    using Dasync.Collections;

    using MassTransit;

    using Moq;

    using StoneAssemblies.MassAuth.Services;

    using Xunit;

    /// <summary>
    /// The default bus selector facts.
    /// </summary>
    public class DefaultBusSelectorFacts
    {
        /// <summary>
        /// The constructor method.
        /// </summary>
        public class The_Constructor_Method
        {
            /// <summary>
            /// Throws argument null exception if predicate is null.
            /// </summary>
            [Fact]
            public void Throws_ArgumentNullException_If_Predicate_Is_Null()
            {
                Assert.Throws<ArgumentNullException>(() => new DefaultBusSelector<DemoMessage>(null));
            }

            /// <summary>
            /// Succeeds with none null arguments.
            /// </summary>
            [Fact]
            public void Succeeds_With_None_Null_Arguments()
            {
                var defaultBusSelector = new DefaultBusSelector<DemoMessage>(new Mock<IBus>().Object);
                Assert.NotNull(defaultBusSelector);
            }
        }

        /// <summary>
        /// The select client factories method.
        /// </summary>
        public class The_SelectClientFactories_Method
        {
            /// <summary>
            /// Returns the client factory.
            /// </summary>
            /// <returns>
            /// The <see cref="Task"/>.
            /// </returns>
            [Fact]
            public async Task Returns_The_ClientFactory()
            {
                var defaultBusSelector = new DefaultBusSelector<DemoMessage>(new Mock<IBus>().Object);
                var clientFactories = await defaultBusSelector.SelectClientFactories(new DemoMessage()).ToListAsync();
                Assert.NotEmpty(clientFactories);
            }
        }
    }
}