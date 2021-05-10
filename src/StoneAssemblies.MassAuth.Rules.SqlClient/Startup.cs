// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Startup.cs" company="Stone Assemblies">
// Copyright © 2021 - 2021 Stone Assemblies. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace StoneAssemblies.MassAuth.Rules.SqlClient
{
    using System.Collections.Generic;
    using System.Data;
    using System.Linq;
    using System.Text.RegularExpressions;

    using Microsoft.Data.SqlClient;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;

    using StoneAssemblies.Extensibility.Services.Interfaces;
    using StoneAssemblies.MassAuth.Messages;
    using StoneAssemblies.MassAuth.Rules.SqlClient.Extensions;

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
        ///     The extension manager.
        /// </summary>
        private readonly IExtensionManager extensionManager;

        /// <summary>
        ///     Initializes a new instance of the <see cref="Startup" /> class.
        /// </summary>
        /// <param name="configuration">
        ///     The configuration.
        /// </param>
        /// <param name="extensionManager">
        ///     The extension manager.
        /// </param>
        public Startup(IConfiguration configuration, IExtensionManager extensionManager)
        {
            this.configuration = configuration;
            this.extensionManager = extensionManager;
        }

        /// <summary>
        ///     The configure services.
        /// </summary>
        /// <param name="serviceCollection">
        ///     The service collection.
        /// </param>
        public void ConfigureServices(IServiceCollection serviceCollection)
        {
            this.extensionManager.Finished +=
                (sender, args) => this.RegisterStoredProcedureBasedRules(serviceCollection);
        }

        /// <summary>
        ///     Register stored procedure based rules.
        /// </summary>
        /// <param name="serviceCollection">
        ///     The service collection.
        /// </param>
        private void RegisterStoredProcedureBasedRules(IServiceCollection serviceCollection)
        {
            const string MessageTypeCapturingGroupName = "messageType";
            var regexOptions = RegexOptions.IgnoreCase | RegexOptions.Multiline | RegexOptions.Compiled;

            var patterns = new List<string>();
            var configurationSection = this.configuration.GetSection("SqlClientStoredProcedureBasedRules");

            configurationSection?.GetSection("Patterns")?.Bind(patterns);
            var regularExpressions = patterns.Select(pattern => new Regex(pattern, regexOptions)).ToList();

            if (regularExpressions.Count == 0)
            {
                return;
            }

            var connectionStrings = new List<string>();
            configurationSection?.GetSection("ConnectionStrings")?.Bind(connectionStrings);
            foreach (var connectionString in connectionStrings)
            {
                using var sqlConnection = new SqlConnection(connectionString);
                var sqlCommand = sqlConnection.CreateCommand();
                sqlCommand.CommandText = "SELECT * FROM INFORMATION_SCHEMA.ROUTINES WHERE ROUTINE_TYPE = 'PROCEDURE'";
                try
                {
                    sqlConnection.Open();
                    var sqlDataReader = sqlCommand.ExecuteReader();
                    while (sqlDataReader.Read())
                    {
                        var storeProcedureName = sqlDataReader.GetString(2);
                        var match = regularExpressions.Select(r => r.Match(storeProcedureName))
                            .FirstOrDefault(m => m.Success);

                        if (match != null && match.Groups.ContainsKey(MessageTypeCapturingGroupName))
                        {
                            var messageTypeName = match.Groups[MessageTypeCapturingGroupName].Value;
                            var messageType = this.extensionManager.GetExtensionAssemblies()
                                .SelectMany(assembly => assembly.GetTypes()).FirstOrDefault(
                                    type => typeof(MessageBase).IsAssignableFrom(type) && type.Name == messageTypeName);
                            if (messageType != null)
                            {
                                serviceCollection.RegisterStoredProcedureBasedRule(
                                    messageType,
                                    connectionString,
                                    storeProcedureName);
                            }
                        }
                    }
                }
                finally
                {
                    if (sqlConnection.State == ConnectionState.Open)
                    {
                        sqlConnection.Close();
                    }
                }
            }
        }
    }
}