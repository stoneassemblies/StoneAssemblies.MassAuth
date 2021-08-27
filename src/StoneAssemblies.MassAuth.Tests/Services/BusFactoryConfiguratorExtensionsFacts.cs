// --------------------------------------------------------------------------------------------------------------------
// <copyright file="BusFactoryConfiguratorExtensionsFacts.cs" company="Stone Assemblies">
// Copyright © 2021 - 2021 Stone Assemblies. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace StoneAssemblies.MassAuth.Tests.Services
{
    using System;

    using MassTransit;
    using MassTransit.ExtensionsDependencyInjectionIntegration;

    using Moq;

    using StoneAssemblies.MassAuth.Bank.Messages;
    using StoneAssemblies.MassAuth.Messages;
    using StoneAssemblies.MassAuth.Messages.Extensions;
    using StoneAssemblies.MassAuth.Services.Extensions;

    using Xunit;

    /// <summary>
    ///     The bus factory configurator extensions facts.
    /// </summary>
    public class BusFactoryConfiguratorExtensionsFacts
    {
        /// <summary>
        ///     The the add default authorization request client method.
        /// </summary>
        public class The_AddDefaultAuthorizationRequestClient_Method
        {
            /// <summary>
            ///     The calls the add request client of the configurator for authorization request message for the given message.
            /// </summary>
            [Fact]
            public void Calls_The_AddRequestClient_Of_The_Configurator_For_AuthorizationRequestMessage_For_The_Given_Message()
            {
                var serviceCollectionBusConfiguratorMock = new Mock<IServiceCollectionBusConfigurator>();
                var serviceCollectionBusConfigurator = serviceCollectionBusConfiguratorMock.Object;

                serviceCollectionBusConfigurator.AddDefaultAuthorizationRequestClient<AccountBalanceRequestMessage>();

                serviceCollectionBusConfiguratorMock.Verify(
                    configurator => configurator.AddRequestClient<AuthorizationRequestMessage<AccountBalanceRequestMessage>>(
                        It.Is<Uri>(
                            uri => uri.ToString()
                                   == $"queue:{typeof(AuthorizationRequestMessage<AccountBalanceRequestMessage>).GetFlatName()}"),
                        It.IsAny<RequestTimeout>()));
            }
        }

        /// <summary>
        ///     The the add default request client method.
        /// </summary>
        public class The_AddDefaultRequestClient_Method
        {
            /// <summary>
            ///     The calls the add request client of the configurator for the given message.
            /// </summary>
            [Fact]
            public void Calls_The_AddRequestClient_Of_The_Configurator_For_The_Given_Message()
            {
                var serviceCollectionBusConfiguratorMock = new Mock<IServiceCollectionBusConfigurator>();
                var serviceCollectionBusConfigurator = serviceCollectionBusConfiguratorMock.Object;

                serviceCollectionBusConfigurator.AddDefaultRequestClient<AccountBalanceRequestMessage>();

                serviceCollectionBusConfiguratorMock.Verify(
                    configurator => configurator.AddRequestClient<AccountBalanceRequestMessage>(
                        It.Is<Uri>(uri => uri.ToString() == $"queue:{typeof(AccountBalanceRequestMessage).GetFlatName()}"),
                        It.IsAny<RequestTimeout>()));
            }
        }
    }
}