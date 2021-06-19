namespace StoneAssemblies.MassAuth.Tests.Hosting.Services
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    using MassTransit.Testing;

    using Moq;

    using StoneAssemblies.MassAuth.Bank.Messages;
    using StoneAssemblies.MassAuth.Hosting.Services;
    using StoneAssemblies.MassAuth.Messages;
    using StoneAssemblies.MassAuth.Rules.Interfaces;

    using Xunit;

    /// <summary>
    /// The authorization message consumer facts.
    /// </summary>
    public class AuthorizationMessageConsumerFacts
    {
        /// <summary>
        /// Returns false in authorized when any rule returns fail by error async.
        /// </summary>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        [Fact]
        public async Task Returns_False_In_Authorized_When_Any_Rule_Returns_Fail_By_Error_Async()
        {
            var harness = new InMemoryTestHarness();
            var ruleA = new Mock<IRule<AuthorizationRequestMessage<AccountBalanceRequestMessage>>>();
            ruleA.SetupGet(rule => rule.IsEnabled).Returns(true);
            ruleA.Setup(
                    rule => rule.EvaluateAsync(It.IsAny<AuthorizationRequestMessage<AccountBalanceRequestMessage>>()))
                .ReturnsAsync(true);
            var ruleB = new Mock<IRule<AuthorizationRequestMessage<AccountBalanceRequestMessage>>>();
            ruleB.SetupGet(rule => rule.IsEnabled).Returns(true);
            ruleB.Setup(
                    rule => rule.EvaluateAsync(It.IsAny<AuthorizationRequestMessage<AccountBalanceRequestMessage>>()))
                .Throws<Exception>();

            var rules = new List<IRule<AuthorizationRequestMessage<AccountBalanceRequestMessage>>>
                            {
                                ruleA.Object,
                                ruleB.Object
                            };

            var rulesContainer = new RulesContainer<AuthorizationRequestMessage<AccountBalanceRequestMessage>>(rules);

            harness.Consumer(
                () =>
                    new AuthorizationRequestMessageConsumer<AuthorizationRequestMessage<AccountBalanceRequestMessage>>(
                        rulesContainer));

            await harness.Start();
            try
            {
                var requestClient =
                    harness.CreateRequestClient<AuthorizationRequestMessage<AccountBalanceRequestMessage>>();
                var response = await requestClient.GetResponse<AuthorizationResponseMessage>(
                                   new AuthorizationRequestMessage<AccountBalanceRequestMessage>());
                Assert.False(response.Message.IsAuthorized);
            }
            finally
            {
                await harness.Stop();
            }
        }

        /// <summary>
        /// The returns false in authorized when any rule returns false async.
        /// </summary>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        [Fact]
        public async Task Returns_False_In_Authorized_When_Any_Rule_Returns_False_Async()
        {
            var harness = new InMemoryTestHarness();
            var ruleA = new Mock<IRule<AuthorizationRequestMessage<AccountBalanceRequestMessage>>>();
            ruleA.SetupGet(rule => rule.IsEnabled).Returns(true);
            ruleA.Setup(
                    rule => rule.EvaluateAsync(It.IsAny<AuthorizationRequestMessage<AccountBalanceRequestMessage>>()))
                .ReturnsAsync(true);
            var ruleB = new Mock<IRule<AuthorizationRequestMessage<AccountBalanceRequestMessage>>>();
            ruleB.SetupGet(rule => rule.IsEnabled).Returns(true);
            ruleB.Setup(
                    rule => rule.EvaluateAsync(It.IsAny<AuthorizationRequestMessage<AccountBalanceRequestMessage>>()))
                .ReturnsAsync(false);

            var rules = new List<IRule<AuthorizationRequestMessage<AccountBalanceRequestMessage>>>
                            {
                                ruleA.Object,
                                ruleB.Object
                            };

            var rulesContainer = new RulesContainer<AuthorizationRequestMessage<AccountBalanceRequestMessage>>(rules);

            harness.Consumer(
                () =>
                    new AuthorizationRequestMessageConsumer<AuthorizationRequestMessage<AccountBalanceRequestMessage>>(
                        rulesContainer));

            await harness.Start();
            try
            {
                var requestClient =
                    harness.CreateRequestClient<AuthorizationRequestMessage<AccountBalanceRequestMessage>>();
                var response = await requestClient.GetResponse<AuthorizationResponseMessage>(
                                   new AuthorizationRequestMessage<AccountBalanceRequestMessage>());
                Assert.False(response.Message.IsAuthorized);
            }
            finally
            {
                await harness.Stop();
            }
        }

        /// <summary>
        /// Returns true in authorized when all rules returns true async.
        /// </summary>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        [Fact]
        public async Task Returns_True_In_Authorized_When_All_Rules_Returns_True_Async()
        {
            var harness = new InMemoryTestHarness();
            var ruleA = new Mock<IRule<AuthorizationRequestMessage<AccountBalanceRequestMessage>>>();
            ruleA.SetupGet(rule => rule.IsEnabled).Returns(true);
            ruleA.Setup(
                    rule => rule.EvaluateAsync(It.IsAny<AuthorizationRequestMessage<AccountBalanceRequestMessage>>()))
                .ReturnsAsync(true);
            var ruleB = new Mock<IRule<AuthorizationRequestMessage<AccountBalanceRequestMessage>>>();
            ruleB.SetupGet(rule => rule.IsEnabled).Returns(true);
            ruleB.Setup(
                    rule => rule.EvaluateAsync(It.IsAny<AuthorizationRequestMessage<AccountBalanceRequestMessage>>()))
                .ReturnsAsync(true);

            var rules = new List<IRule<AuthorizationRequestMessage<AccountBalanceRequestMessage>>>
                            {
                                ruleA.Object,
                                ruleB.Object
                            };

            var rulesContainer = new RulesContainer<AuthorizationRequestMessage<AccountBalanceRequestMessage>>(rules);

            harness.Consumer(
                () =>
                    new AuthorizationRequestMessageConsumer<AuthorizationRequestMessage<AccountBalanceRequestMessage>>(
                        rulesContainer));

            await harness.Start();
            try
            {
                var requestClient =
                    harness.CreateRequestClient<AuthorizationRequestMessage<AccountBalanceRequestMessage>>();
                var response = await requestClient.GetResponse<AuthorizationResponseMessage>(
                                   new AuthorizationRequestMessage<AccountBalanceRequestMessage>());

                Assert.True(response.Message.IsAuthorized);
            }
            finally
            {
                await harness.Stop();
            }
        }

        /// <summary>
        /// Runs enabled rules only async.
        /// </summary>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        [Fact]
        public async Task Runs_Enabled_Rules_Only_Async()
        {
            var harness = new InMemoryTestHarness();
            var ruleA = new Mock<IRule<AuthorizationRequestMessage<AccountBalanceRequestMessage>>>();
            ruleA.SetupGet(rule => rule.IsEnabled).Returns(false);
            ruleA.Setup(
                    rule => rule.EvaluateAsync(It.IsAny<AuthorizationRequestMessage<AccountBalanceRequestMessage>>()))
                .ReturnsAsync(true);
            var ruleB = new Mock<IRule<AuthorizationRequestMessage<AccountBalanceRequestMessage>>>();
            ruleB.SetupGet(rule => rule.IsEnabled).Returns(true);
            ruleB.Setup(
                    rule => rule.EvaluateAsync(It.IsAny<AuthorizationRequestMessage<AccountBalanceRequestMessage>>()))
                .ReturnsAsync(false);

            var rules = new List<IRule<AuthorizationRequestMessage<AccountBalanceRequestMessage>>>
                            {
                                ruleA.Object,
                                ruleB.Object
                            };

            var rulesContainer = new RulesContainer<AuthorizationRequestMessage<AccountBalanceRequestMessage>>(rules);

            harness.Consumer(
                () =>
                    new AuthorizationRequestMessageConsumer<AuthorizationRequestMessage<AccountBalanceRequestMessage>>(
                        rulesContainer));

            await harness.Start();
            try
            {
                var requestClient =
                    harness.CreateRequestClient<AuthorizationRequestMessage<AccountBalanceRequestMessage>>();
                await requestClient.GetResponse<AuthorizationResponseMessage>(
                    new AuthorizationRequestMessage<AccountBalanceRequestMessage>());

                ruleA.Verify(
                    rule => rule.EvaluateAsync(It.IsAny<AuthorizationRequestMessage<AccountBalanceRequestMessage>>()),
                    Times.Never);
                ruleB.Verify(
                    rule => rule.EvaluateAsync(It.IsAny<AuthorizationRequestMessage<AccountBalanceRequestMessage>>()),
                    Times.Once);
            }
            finally
            {
                await harness.Stop();
            }
        }
    }
}