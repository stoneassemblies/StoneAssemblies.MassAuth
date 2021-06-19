// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Startup.cs" company="Stone Assemblies">
// Copyright � 2021 - 2021 Stone Assemblies. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace StoneAssemblies.MassAuth.Rules.SqlClient
{
    using System.Collections.Generic;

    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;

    using StoneAssemblies.MassAuth.Rules.SqlClient.Extensions;
    using StoneAssemblies.MassAuth.Rules.SqlClient.Services;

    /// <summary>
    ///     The startup.
    /// </summary>
    public class Startup
    {
        /// <summary>
        ///     The configuration.
        /// </summary>
        private readonly IConfiguration configuration;

        /// <summary>
        ///     Initializes a new instance of the <see cref="Startup" /> class.
        /// </summary>
        /// <param name="configuration">
        ///     The configuration.
        /// </param>
        public Startup(IConfiguration configuration)
        {
            this.configuration = configuration;
        }

        /// <summary>
        ///     The configure services.
        /// </summary>
        /// <param name="serviceCollection">
        ///     The service collection.
        /// </param>
        public void ConfigureServices(IServiceCollection serviceCollection)
        {
            var configurationSection = this.configuration?.GetSection("SqlClientStoredProcedureBasedRules");

            var patterns = new List<string>();
            configurationSection?.GetSection("Patterns")?.Bind(patterns);

            var connectionStrings = new List<string>();
            configurationSection?.GetSection("ConnectionStrings")?.Bind(connectionStrings);
            foreach (var connectionString in connectionStrings)
            {
                var sqlServerDatabaseInspector = new SqlClientDatabaseInspector(connectionString);
                serviceCollection.RegisterStoredProcedureBasedRules(sqlServerDatabaseInspector, patterns);
            }
        }
    }
}