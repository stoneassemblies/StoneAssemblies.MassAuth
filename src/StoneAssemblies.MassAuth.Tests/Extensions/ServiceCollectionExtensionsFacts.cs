// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ServiceCollectionExtensionsFacts.cs" company="Stone Assemblies">
// Copyright © 2021 - 2021 Stone Assemblies. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace StoneAssemblies.MassAuth.Tests.Extensions
{
    using System.Linq;
    using System.Threading.Tasks;

    using MassTransit;

    using Microsoft.Extensions.DependencyInjection;

    using StoneAssemblies.MassAuth.Extensions;
    using StoneAssemblies.MassAuth.Services.Interfaces;

    using Xunit;

    /// <summary>
    ///     The service collection extensions facts.
    /// </summary>
    public class ServiceCollectionExtensionsFacts
    {
        public class The_AddBusSelector_Method
        {
            /// <summary>
            /// The registers_ bus selector.
            /// </summary>
            [Fact]
            public async Task Registers_BusSelector()
            {
                var serviceCollection = new ServiceCollection();
                serviceCollection.AddMassTransit("FourBus", cfg => { cfg.UsingInMemory(); });
                serviceCollection.AddMassTransit("FiveBus", cfg => { cfg.UsingInMemory(); });

                serviceCollection.AddBusSelector<DemoMessage>((b, m) =>
                    {
                        if (b.GetType().Name == "FourBus")
                        {
                            return Task.FromResult(true);
                        }

                        return Task.FromResult(false);
                    });

                var busSelector = serviceCollection.BuildServiceProvider().GetService<IBusSelector<DemoMessage>>();

                Assert.NotNull(busSelector);
            }
        }

        /// <summary>
        ///     The the add mass transit method.
        /// </summary>
        public class The_AddMassTransit_Method
        {
            /// <summary>
            ///     The invokes the configuration action.
            /// </summary>
            [Fact]
            public void Invokes_The_Configuration_Action()
            {
                var invoked = false;
                var serviceCollection = new ServiceCollection();
                serviceCollection.AddMassTransit(
                    "ThirdBus",
                    configurator =>
                        {
                            configurator.UsingInMemory();
                            invoked = true;
                        });
                Assert.True(invoked);
            }

            /// <summary>
            ///     The registers the generated bus type.
            /// </summary>
            [Fact]
            public void Registers_The_Generated_BusType()
            {
                var serviceCollection = new ServiceCollection();
                serviceCollection.AddMassTransit("SecondBus", configurator => configurator.UsingInMemory());
                var serviceDescriptor = serviceCollection.FirstOrDefault(
                    descriptor => descriptor.ServiceType.GetGenericArguments().Length > 0
                                  && descriptor.ServiceType.GetGenericArguments()[0].Name == "SecondBus");

                Assert.NotNull(serviceDescriptor);
            }
        }
    }
}