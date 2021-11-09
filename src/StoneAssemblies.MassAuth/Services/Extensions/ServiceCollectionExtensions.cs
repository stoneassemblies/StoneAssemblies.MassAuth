// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ServiceCollectionExtensions.cs" company="Stone Assemblies">
// Copyright © 2021 - 2021 Stone Assemblies. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace StoneAssemblies.MassAuth.Services.Extensions
{
    using System;

    using Microsoft.Extensions.DependencyInjection;

    using StoneAssemblies.MassAuth.Services.Options;

    /// <summary>
    ///     The service collection extensions.
    /// </summary>
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        ///     Adds MassAuth services.
        /// </summary>
        /// <param name="serviceCollection">
        ///     The service collection.
        /// </param>
        /// <param name="configure">
        ///     The configuration options.
        /// </param>
        public static void AddMassAuth(this IServiceCollection serviceCollection, Action<AuthorizeByRuleFilterConfigurationOptions> configure = null)
        {
            var authorizeByRuleFilterOptions = new AuthorizeByRuleFilterConfigurationOptions();
            configure?.Invoke(authorizeByRuleFilterOptions);

            serviceCollection.AddScoped(provider => authorizeByRuleFilterOptions);
            serviceCollection.AddScoped<AuthorizeByRuleFilter>();
        }
    }
}