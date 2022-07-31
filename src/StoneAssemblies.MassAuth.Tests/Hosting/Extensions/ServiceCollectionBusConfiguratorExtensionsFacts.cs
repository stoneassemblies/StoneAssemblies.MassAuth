// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ServiceCollectionBusConfiguratorExtensionsFacts.cs" company="Stone Assemblies">
// Copyright © 2021 - 2021 Stone Assemblies. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace StoneAssemblies.MassAuth.Tests.Hosting.Extensions
{
    using System;

    using MassTransit;
    using MassTransit.Configuration;

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
                var consumerRegistrationMock = new Mock<IConsumerRegistrationConfigurator<IConsumer<AuthorizationRequestMessage<AccountBalanceRequestMessage>>>>();

                var serviceCollection = new ServiceCollection();

                var registrationConfiguratorStub = new RegistrationConfiguratorStub<IConsumer<AuthorizationRequestMessage<AccountBalanceRequestMessage>>>(serviceCollection, new Mock<IContainerRegistrar>().Object, consumerRegistrationMock.Object);

                serviceCollection.GetDiscoveredMessageTypes().Add(typeof(AuthorizationRequestMessage<AccountBalanceRequestMessage>));

                registrationConfiguratorStub.AddAuthorizationRequestConsumers();

                // consumerRegistrationMock.Verify(configurator => configurator);

                //consumerRegistrationMock.Verify(
                //    configurator => configurator.AddConsumer(
                //        typeof(AuthorizationRequestMessageConsumer<AuthorizationRequestMessage<AccountBalanceRequestMessage>>),
                //        null),
                //    Times.Once());
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
                var serviceCollection = new ServiceCollection();

                var consumerRegistrationMock = new Mock<IConsumerRegistrationConfigurator<IConsumer<AuthorizationRequestMessage<AccountBalanceRequestMessage>>>>();

                var registrationConfiguratorStub = new RegistrationConfiguratorStub<IConsumer<AuthorizationRequestMessage<AccountBalanceRequestMessage>>>(serviceCollection, new Mock<IContainerRegistrar>().Object, consumerRegistrationMock.Object);

                serviceCollection.GetDiscoveredMessageTypes().Add(typeof(AuthorizationRequestMessage<AccountBalanceRequestMessage>));

                registrationConfiguratorStub.AddAuthorizationRequestConsumers();

                var invoked = false;
                registrationConfiguratorStub.ConfigureAuthorizationRequestConsumers(
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

    public class RegistrationConfiguratorStub<T1> : RegistrationConfigurator, IBusRegistrationConfigurator
        where T1 : class, IConsumer
    {
        private readonly IConsumerRegistrationConfigurator<T1> consumerRegistrationConfigurator;

        public RegistrationConfiguratorStub(
            IServiceCollection collection, IContainerRegistrar registrar,
            IConsumerRegistrationConfigurator<T1> consumerRegistrationConfigurator)
            : base(collection, registrar)
        {
            this.consumerRegistrationConfigurator = consumerRegistrationConfigurator;
        }

        public void AddBus(Func<IBusRegistrationContext, IBusControl> busFactory)
        {
            throw new NotImplementedException();
        }

        public void SetBusFactory<T>(T busFactory)
            where T : IRegistrationBusFactory
        {
            throw new NotImplementedException();
        }

        public void AddRider(Action<IRiderRegistrationConfigurator> configure)
        {
            throw new NotImplementedException();
        }

        public IConsumerRegistrationConfigurator<T> AddConsumer<T>(Action<IConsumerConfigurator<T>> configure = null)
            where T : class, IConsumer
        {
            throw new NotImplementedException();
        }

        public IConsumerRegistrationConfigurator<T> AddConsumer<T>(Type consumerDefinitionType, Action<IConsumerConfigurator<T>> configure = null)
            where T : class, IConsumer
        {
            return this.consumerRegistrationConfigurator as IConsumerRegistrationConfigurator<T>;
        }
    }
}