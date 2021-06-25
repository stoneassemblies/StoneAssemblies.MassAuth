// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ServiceCollectionBusConfiguratorExtensionsFacts.cs" company="Stone Assemblies">
// Copyright © 2021 - 2021 Stone Assemblies. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace StoneAssemblies.MassAuth.Tests.Hosting.Extensions
{
    using MassTransit.ExtensionsDependencyInjectionIntegration;

    using Microsoft.Extensions.DependencyInjection;

    using Moq;

    using StoneAssemblies.MassAuth.Bank.Messages;
    using StoneAssemblies.MassAuth.Hosting.Extensions;
    using StoneAssemblies.MassAuth.Hosting.Services;
    using StoneAssemblies.MassAuth.Messages;

    using Xunit;

    /// <summary>
    ///     The service collection bus configurator extensions facts.
    /// </summary>
    public class ServiceCollectionBusConfiguratorExtensionsFacts
    {
        /// <summary>
        ///     The add authorization request consumers_ method.
        /// </summary>
        public class The_AddAuthorizationRequestConsumers_Method
        {
            /// <summary>
            ///     Adds consumer to the service collection bus configurator.
            /// </summary>
            [Fact]
            public void Adds_Consumer_To_The_ServiceCollectionBusConfigurator()
            {
                var serviceCollectionBusConfiguratorMock = new Mock<IServiceCollectionBusConfigurator>();

                var serviceCollection = new ServiceCollection();
                serviceCollection.GetDiscoveredMessageTypes().Add(typeof(AuthorizationRequestMessage<AccountBalanceRequestMessage>));

                serviceCollectionBusConfiguratorMock.SetupGet(configurator => configurator.Collection).Returns(serviceCollection);

                serviceCollectionBusConfiguratorMock.Object.AddAuthorizationRequestConsumers();

                serviceCollectionBusConfiguratorMock.Verify(
                    configurator => configurator.AddConsumer(
                        typeof(AuthorizationRequestMessageConsumer<AuthorizationRequestMessage<AccountBalanceRequestMessage>>),
                        null),
                    Times.Once());
            }
        }

        /// <summary>
        ///     The configure authorization request consumers_ method.
        /// </summary>
        public class The_ConfigureAuthorizationRequestConsumers_Method
        {
            /// <summary>
            ///     Invokes configure action for each registered consumer.
            /// </summary>
            [Fact]
            public void Invokes_Configure_Action_ForEach_Registered_Consumer()
            {
                var serviceCollectionBusConfiguratorMock = new Mock<IServiceCollectionBusConfigurator>();

                var serviceCollection = new ServiceCollection();
                serviceCollection.GetDiscoveredMessageTypes().Add(typeof(AuthorizationRequestMessage<AccountBalanceRequestMessage>));

                serviceCollectionBusConfiguratorMock.SetupGet(configurator => configurator.Collection).Returns(serviceCollection);
                serviceCollectionBusConfiguratorMock.Object.AddAuthorizationRequestConsumers();

                var invoked = false;
                serviceCollectionBusConfiguratorMock.Object.ConfigureAuthorizationRequestConsumers(
                    (messageType, consumerType) =>
                        {
                            invoked = true;
                            Assert.Equal(typeof(AuthorizationRequestMessage<AccountBalanceRequestMessage>), messageType);
                            Assert.Equal(
                                typeof(AuthorizationRequestMessageConsumer<AuthorizationRequestMessage<AccountBalanceRequestMessage>>),
                                consumerType);
                        });

                Assert.True(invoked);
            }
        }
    }
}