// --------------------------------------------------------------------------------------------------------------------
// <copyright file="BusSelectorPredicateFacts.cs" company="Stone Assemblies">
// Copyright © 2021 - 2021 Stone Assemblies. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace StoneAssemblies.MassAuth.Tests.Services
{
    using System;
    using System.Threading.Tasks;

    using MassTransit;

    using Moq;

    using StoneAssemblies.MassAuth.Services;

    using Xunit;

    /// <summary>
    ///     The bus selector predicate facts.
    /// </summary>
    public class BusSelectorPredicateFacts
    {
        /// <summary>
        ///     The constructor method.
        /// </summary>
        public class The_Constructor_Method
        {
            /// <summary>
            ///     The succeeds with none null arguments.
            /// </summary>
            [Fact]
            public void Succeeds_With_None_Null_Arguments()
            {
                var busSelector = new BusSelectorPredicate<DemoMessage>((bus, message) => Task.FromResult(true));
                Assert.NotNull(busSelector);
            }

            /// <summary>
            ///     The throws argument null exception if predicate is_ null.
            /// </summary>
            /// <returns>
            ///     The <see cref="Task" />.
            /// </returns>
            [Fact]
            public void Throws_ArgumentNullException_If_Predicate_Is_Null()
            {
                Assert.Throws<ArgumentNullException>(() => new BusSelectorPredicate<DemoMessage>(null));
            }
        }

        /// <summary>
        ///     The the is match method.
        /// </summary>
        public class The_IsMatch_Method
        {
            /// <summary>
            ///     The returns false if the predicate returns false.
            /// </summary>
            /// <returns>
            ///     The <see cref="Task" />.
            /// </returns>
            [Fact]
            public async Task Returns_False_If_The_Predicate_Returns_False()
            {
                var busSelectorPredicate = new BusSelectorPredicate<DemoMessage>((bus, message) => Task.FromResult(false));
                var isMatch = await busSelectorPredicate.IsMatch(new Mock<IBus>().Object, new DemoMessage());
                Assert.False(isMatch);
            }

            /// <summary>
            ///     The returns true if the predicate returns true.
            /// </summary>
            /// <returns>
            ///     The <see cref="Task" />.
            /// </returns>
            [Fact]
            public async Task Returns_True_If_The_Predicate_Returns_True()
            {
                var busSelectorPredicate = new BusSelectorPredicate<DemoMessage>((bus, message) => Task.FromResult(true));
                var isMatch = await busSelectorPredicate.IsMatch(new Mock<IBus>().Object, new DemoMessage());
                Assert.True(isMatch);
            }
        }
    }
}