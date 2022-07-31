// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Startup.cs" company="Stone Assemblies">
// Copyright © 2021 - 2021 Stone Assemblies. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace StoneAssemblies.MassAuth.Bank.Rules
{
    using FluentValidation;

    using Microsoft.AspNetCore.Identity;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;

    using StoneAssemblies.MassAuth.Bank.Messages;
    using StoneAssemblies.MassAuth.Messages;

    /// <summary>
    ///     The startup type.
    /// </summary>
    public class Startup
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="Startup" /> class.
        /// </summary>
        /// <param name="configuration">
        ///     The configuration.
        /// </param>
        public Startup(IConfiguration configuration)
        {
            this.Configuration = configuration;
        }

        /// <summary>
        ///     Gets the configuration.
        /// </summary>
        public IConfiguration Configuration { get; }

        /// <summary>
        ///     The configure services.
        /// </summary>
        /// <param name="serviceCollection">
        ///     The service collection.
        /// </param>
        public void ConfigureServices(IServiceCollection serviceCollection)
        {
        }
    }
}