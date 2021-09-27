// --------------------------------------------------------------------------------------------------------------------
// <copyright file="BusFactoryConfiguratorExtensionsFacts.cs" company="Stone Assemblies">
// Copyright © 2021 - 2021 Stone Assemblies. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace StoneAssemblies.MassAuth.Tests.Hosting.Extensions
{
    using System;

    using MassTransit;

    using Moq;

    using StoneAssemblies.Contrib.MassTransit.Extensions;
    using StoneAssemblies.MassAuth.Bank.Messages;
    using StoneAssemblies.MassAuth.Hosting.Extensions;

    using Xunit;

    /// <summary>
    ///     The bus factory configurator extensions facts.
    /// </summary>
    public class BusFactoryConfiguratorExtensionsFacts
    {
        /// <summary>
        ///     The default receive endpoint method.
        /// </summary>
        public class The_DefaultReceiveEndpoint_Method
        {
            /// <summary>
            ///     Calls the receive endpoint of the configurator with a queue name based on the message type.
            /// </summary>
            [Fact]
            public void Calls_The_ReceiveEndpoint_Of_The_Configurator_With_A_QueueName_Based_On_The_MessageType()
            {
                var busConfiguratorMock = new Mock<IBusFactoryConfigurator>();
                var serviceCollectionBusConfigurator = busConfiguratorMock.Object;

                serviceCollectionBusConfigurator.DefaultReceiveEndpoint<AccountBalanceRequestMessage>(configurator => { });

                busConfiguratorMock.Verify(
                    configurator => configurator.ReceiveEndpoint(
                        It.Is<string>(queueName => queueName == typeof(AccountBalanceRequestMessage).GetFlatName()),
                        It.IsAny<Action<IReceiveEndpointConfigurator>>()));
            }
        }
    }
}