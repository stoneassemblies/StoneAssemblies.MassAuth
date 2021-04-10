﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ServiceCollectionExtensions.cs" company="Stone Assemblies">
// Copyright © 2021 - 2021 Stone Assemblies. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace StoneAssemblies.MassAuth.Hosting.Extensions
{
    using System.Linq;

    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;

    using StoneAssemblies.MassAuth.Hosting.Services;

    /// <summary>
    ///     The service collection extensions.
    /// </summary>
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        ///     Add the extension manager.
        /// </summary>
        /// <param name="serviceCollection">
        ///     The service collection.
        /// </param>
        /// <param name="configuration">
        ///     The configuration.
        /// </param>
        /// <param name="load">
        ///     The load.
        /// </param>
        public static void AddExtensionManager(
            this IServiceCollection serviceCollection,
            IConfiguration configuration,
            bool load = false)
        {
            var extensionManager = ExtensionManager.From(configuration);
            serviceCollection.AddSingleton(extensionManager);
            if (load)
            {
                extensionManager.LoadExtensionsAsync().GetAwaiter().GetResult();
            }
        }

        /// <summary>
        ///     Adds service discovery.
        /// </summary>
        /// <param name="serviceCollection">
        ///     The service collection.
        /// </param>
        public static void AddServiceDiscovery(this IServiceCollection serviceCollection)
        {
            var serviceDiscovery = ServiceDiscoveryFactory.GetServiceDiscovery();
            serviceCollection.AddSingleton(serviceDiscovery);
        }

        /// <summary>
        ///     Gets a registered instance.
        /// </summary>
        /// <param name="services">
        ///     The services.
        /// </param>
        /// <typeparam name="TServiceType">
        ///     The service type.
        /// </typeparam>
        /// <returns>
        ///     A registered instance of <typeparamref name="TServiceType" />.
        /// </returns>
        public static TServiceType GetRegisteredInstance<TServiceType>(this IServiceCollection services)
        {
            var type = typeof(TServiceType);
            var serviceDescriptor = services.FirstOrDefault(
                descriptor => descriptor.ServiceType == type && descriptor.ImplementationInstance != null);
            var implementationInstance = serviceDescriptor?.ImplementationInstance;

            return (TServiceType)implementationInstance;
        }
    }
}