// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ServiceCollectionExtensions.cs" company="Stone Assemblies">
// Copyright © 2021 - 2021 Stone Assemblies. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace StoneAssemblies.MassAuth.Rules.SqlClient.Extensions
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Linq;
    using System.Text.RegularExpressions;

    using Microsoft.Data.SqlClient;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;

    using StoneAssemblies.MassAuth.Messages;
    using StoneAssemblies.MassAuth.Rules.Interfaces;
    using StoneAssemblies.MassAuth.Rules.SqlClient.Rules;

    /// <summary>
    ///     The service collection extensions.
    /// </summary>
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        ///     The register stored procedure based rule.
        /// </summary>
        /// <param name="serviceCollection">
        ///     The service collection.
        /// </param>
        /// <param name="messageType">
        ///     The message type.
        /// </param>
        /// <param name="ruleName">
        ///     The rule name.
        /// </param>
        /// <param name="connectionString">
        ///     The connection string.
        /// </param>
        /// <param name="storedProcedureName">
        ///     The store procedure name.
        /// </param>
        public static void RegisterStoredProcedureBasedRule(
            this IServiceCollection serviceCollection,
            Type messageType,
            string ruleName,
            string connectionString,
            string storedProcedureName)
        {
            var authorizationRequestMessageType = typeof(AuthorizationRequestMessage<>).MakeGenericType(messageType);
            var ruleInterfaceType = typeof(IRule<>).MakeGenericType(authorizationRequestMessageType);
            var ruleType = typeof(StoredProcedureBasedRule<>).MakeGenericType(authorizationRequestMessageType);
            serviceCollection.AddSingleton(
                ruleInterfaceType,
                sp => Activator.CreateInstance(ruleType, ruleName, connectionString, storedProcedureName));
        }

        /// <summary>
        ///     Register stored procedure based rules.
        /// </summary>
        /// <param name="serviceCollection">
        ///     The service collection.
        /// </param>
        /// <param name="configuration">
        ///     The configuration.
        /// </param>
        public static void RegisterStoredProcedureBasedRules(
            this IServiceCollection serviceCollection,
            IConfiguration configuration)
        {
            const string MessageTypeCapturingGroupName = "messageType";
            const string RuleNameCapturingGroupName = "ruleName";

            var regexOptions = RegexOptions.IgnoreCase | RegexOptions.Multiline | RegexOptions.Compiled;

            var patterns = new List<string>();
            var configurationSection = configuration?.GetSection("SqlClientStoredProcedureBasedRules");

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
                            var messageType = AppDomain.CurrentDomain.GetAssemblies()
                                .SelectMany(assembly => assembly.GetTypes()).FirstOrDefault(
                                    type => typeof(MessageBase).IsAssignableFrom(type) && type.Name == messageTypeName);


                            string ruleName = string.Empty;
                            if (match.Groups.ContainsKey(RuleNameCapturingGroupName))
                            {
                                ruleName = match.Groups[RuleNameCapturingGroupName].Value;
                            }

                            if (messageType != null)
                            {
                                serviceCollection.RegisterStoredProcedureBasedRule(
                                    messageType,
                                    ruleName,
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