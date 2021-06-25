namespace StoneAssemblies.MassAuth.Tests.Hosting.Services
{
    using System.Collections.Generic;

    using Moq;

    using StoneAssemblies.MassAuth.Bank.Messages;
    using StoneAssemblies.MassAuth.Hosting.Services;
    using StoneAssemblies.MassAuth.Messages;
    using StoneAssemblies.MassAuth.Rules.Interfaces;

    using Xunit;

    /// <summary>
    ///     The rules container facts.
    /// </summary>
    public class RulesContainerFacts
    {
        /// <summary>
        ///     The rules property.
        /// </summary>
        public class The_Rules_Property
        {
            /// <summary>
            ///     Ignores disable rules.
            /// </summary>
            [Fact]
            public void Ignores_Disable_Rules()
            {
                var rules = new List<IRule<AuthorizationRequestMessage<AccountBalanceRequestMessage>>>();

                var ruleAMock = new Mock<IRule<AuthorizationRequestMessage<AccountBalanceRequestMessage>>>();
                ruleAMock.SetupGet(rule => rule.IsEnabled).Returns(true);
                rules.Add(ruleAMock.Object);

                var ruleBMock = new Mock<IRule<AuthorizationRequestMessage<AccountBalanceRequestMessage>>>();
                ruleBMock.SetupGet(rule => rule.IsEnabled).Returns(false);
                rules.Add(ruleBMock.Object);

                var rulesContainer = new RulesContainer<AuthorizationRequestMessage<AccountBalanceRequestMessage>>(rules);

                Assert.Single(rulesContainer.Rules);
            }

            /// <summary>
            ///     Sorts rules by priority.
            /// </summary>
            [Fact]
            public void Sorts_Rules_By_Priority()
            {
                var rules = new List<IRule<AuthorizationRequestMessage<AccountBalanceRequestMessage>>>();

                var ruleAMock = new Mock<IRule<AuthorizationRequestMessage<AccountBalanceRequestMessage>>>();
                ruleAMock.SetupGet(rule => rule.IsEnabled).Returns(true);
                ruleAMock.SetupGet(rule => rule.Priority).Returns(2);
                rules.Add(ruleAMock.Object);

                var ruleBMock = new Mock<IRule<AuthorizationRequestMessage<AccountBalanceRequestMessage>>>();
                ruleBMock.SetupGet(rule => rule.IsEnabled).Returns(true);
                ruleBMock.SetupGet(rule => rule.Priority).Returns(1);
                rules.Add(ruleBMock.Object);

                var ruleCMock = new Mock<IRule<AuthorizationRequestMessage<AccountBalanceRequestMessage>>>();
                ruleCMock.SetupGet(rule => rule.IsEnabled).Returns(true);
                ruleCMock.SetupGet(rule => rule.Priority).Returns(0);
                rules.Add(ruleCMock.Object);

                var ruleDMock = new Mock<IRule<AuthorizationRequestMessage<AccountBalanceRequestMessage>>>();
                ruleDMock.SetupGet(rule => rule.IsEnabled).Returns(true);
                ruleDMock.SetupGet(rule => rule.Priority).Returns(0);
                rules.Add(ruleDMock.Object);

                var rulesContainer = new RulesContainer<AuthorizationRequestMessage<AccountBalanceRequestMessage>>(rules);

                for (var i = 0; i < rulesContainer.Rules.Count - 1; i++)
                {
                    Assert.True(rulesContainer.Rules[i].Priority <= rulesContainer.Rules[i + 1].Priority);
                }
            }
        }
    }
}