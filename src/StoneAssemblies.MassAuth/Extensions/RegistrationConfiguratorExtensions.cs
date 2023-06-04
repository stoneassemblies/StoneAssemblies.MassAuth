// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RegistrationConfiguratorExtensions.cs" company="Stone Assemblies">
// Copyright © 2021 - 2022 Stone Assemblies. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace StoneAssemblies.MassAuth.Extensions
{
    using System.Reflection;

    using MassTransit.Configuration;

    using Microsoft.Extensions.DependencyInjection;

    /// <summary>
    /// The RegistrationConfiguratorExtensions class
    /// </summary>
    public static class RegistrationConfiguratorExtensions
    {
        /// <summary>
        ///     Gets the ServiceCollection
        /// </summary>
        /// <param name="registrationConfigurator">
        ///     The instance.
        /// </param>
        /// <returns>
        ///     The service collection
        /// </returns>
        public static IServiceCollection GetServiceCollection(this RegistrationConfigurator registrationConfigurator)
        {
            var fieldInfo = typeof(RegistrationConfigurator).GetField(
                "_collection",
                BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.FlattenHierarchy);
            return fieldInfo?.GetValue(registrationConfigurator) as IServiceCollection;
        }
    }
}