// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PredicateBasedBusSelector.cs" company="Stone Assemblies">
// Copyright © 2021 - 2021 Stone Assemblies. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace StoneAssemblies.MassAuth.Tests.Services
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    using Dasync.Collections;

    using MassTransit;

    using Microsoft.Extensions.DependencyInjection;

    using Moq;

    using StoneAssemblies.MassAuth.Extensions;
    using StoneAssemblies.MassAuth.Services;
    using StoneAssemblies.MassAuth.Services.Interfaces;

    using Xunit;

    /// <summary>
    ///     The predicate based bus selector.
    /// </summary>
    public class PredicateBasedBusSelector
    {
        /// <summary>
        ///     The constructor method.
        /// </summary>
        public class The_Constructor_Method
        {
            /// <summary>
            ///     Succeeds with none null arguments.
            /// </summary>
            [Fact]
            public void Succeeds_With_None_Null_Arguments()
            {
                var busSelector = new PredicateBasedBusSelector<DemoMessage>(
                    new List<IBus>
                        {
                            new Mock<IBus>().Object
                        },
                    new BusSelectorPredicate<DemoMessage>((bus, message) => Task.FromResult(true)));

                Assert.NotNull(busSelector);
            }

            /// <summary>
            ///     Throws_ argument null exception_ if_ buses_ is_ null.
            /// </summary>
            [Fact]
            public void Throws_ArgumentNullException_If_Buses_Is_Null()
            {
                Assert.Throws<ArgumentNullException>(
                    () => new PredicateBasedBusSelector<DemoMessage>(
                        null,
                        new BusSelectorPredicate<DemoMessage>((bus, message) => Task.FromResult(true))));
            }

            /// <summary>
            ///     Throws_ argument null exception_ if_ predicate_ is_ null.
            /// </summary>
            [Fact]
            public void Throws_ArgumentNullException_If_Predicate_Is_Null()
            {
                Assert.Throws<ArgumentNullException>(() => new PredicateBasedBusSelector<DemoMessage>(null, null));
            }
        }
    }

    /// <summary>
    ///     The select client factories method.
    /// </summary>
    public class The_SelectClientFactories_Method
    {
        /// <summary>
        ///     Does not return the client factories if predicate returns false.
        /// </summary>
        /// <returns>
        ///     The <see cref="Task" />.
        /// </returns>
        [Fact]
        public async Task Does_Not_Return_The_ClientFactories_If_Predicate_Returns_False()
        {
            var bus = new Mock<IBus>().Object;

            var defaultBusSelector = new PredicateBasedBusSelector<DemoMessage>(
                new List<IBus>
                    {
                        bus
                    },
                new BusSelectorPredicate<DemoMessage>((b, m) => Task.FromResult(false)));

            var clientFactories = await defaultBusSelector.SelectClientFactories(new DemoMessage()).ToListAsync();

            Assert.Empty(clientFactories);
        }

        /// <summary>
        ///     Returns the client factory.
        /// </summary>
        /// <returns>
        ///     The <see cref="Task" />.
        /// </returns>
        [Fact]
        public async Task Returns_The_ClientFactory()
        {
            var bus = new Mock<IBus>().Object;

            var defaultBusSelector = new PredicateBasedBusSelector<DemoMessage>(
                new List<IBus>
                    {
                        bus
                    },
                new BusSelectorPredicate<DemoMessage>((b, m) => Task.FromResult(true)));
            var clientFactories = await defaultBusSelector.SelectClientFactories(new DemoMessage()).ToListAsync();
            Assert.NotEmpty(clientFactories);
        }

        [Fact]
        public async Task Returns_The_ClientFactories_According_To_The_Predicate()
        {
            var serviceCollection = new ServiceCollection();
            serviceCollection.AddMassTransit("SixBus", cfg => { cfg.UsingInMemory(); });
            serviceCollection.AddMassTransit("SevenBus", cfg => { cfg.UsingInMemory(); });

            serviceCollection.AddBusSelector<DemoMessage>(
                (b, m) =>
                    {
                        if (b.GetType().GetInterface("SixBus") != null)
                        {
                            return Task.FromResult(true);
                        }

                        return Task.FromResult(false);
                    });

            var busSelector = serviceCollection.BuildServiceProvider().GetService<IBusSelector<DemoMessage>>();

            var clientFactories = await busSelector.SelectClientFactories(new DemoMessage()).ToListAsync();
            Assert.Equal(1, clientFactories?.Count);
        }
    }
}