﻿namespace StoneAssemblies.MassAuth.Tests.Hosting.Extensions
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;

    using Microsoft.Extensions.DependencyInjection;

    using Moq;

    using StoneAssemblies.Extensibility.Services.Interfaces;
    using StoneAssemblies.MassAuth.Bank.Messages;
    using StoneAssemblies.MassAuth.Bank.Rules;
    using StoneAssemblies.MassAuth.Hosting.Extensions;
    using StoneAssemblies.MassAuth.Messages;
    using StoneAssemblies.MassAuth.Rules.Interfaces;

    using Xunit;

    /// <summary>
    ///     The service collection extensions facts.
    /// </summary>
    public class ServiceCollectionExtensionsFacts
    {
        /// <summary>
        ///     The add rules method.
        /// </summary>
        public class The_Add_Rules_Method
        {
            /// <summary>
            ///     Loads rules from the extension assemblies.
            /// </summary>
            [Fact]
            public void Loads_Rules_From_The_Extension_Assemblies()
            {
                var extensionManagerMock = new Mock<IExtensionManager>();
                var assemblies = new List<Assembly>
                                     {
                                         typeof(Startup).Assembly
                                     };

                extensionManagerMock.Setup(manager => manager.GetExtensionAssemblies()).Returns(assemblies);

                var serviceCollection = new ServiceCollection();
                serviceCollection.AddSingleton(extensionManagerMock.Object);
                serviceCollection.AddRules();

                var buildServiceProvider = serviceCollection.BuildServiceProvider();
                var rules = buildServiceProvider
                    .GetServices<IRule<AuthorizationRequestMessage<AccountBalanceRequestMessage>>>().ToList();

                Assert.NotEmpty(rules);
            }
        }
    }
}
