﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ServiceCollectionExtensions.cs" company="Stone Assemblies">
// Copyright © 2021 - 2021 Stone Assemblies. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace StoneAssemblies.MassAuth.Rules.SqlClient.Extensions
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text.RegularExpressions;
    using System.Threading.Tasks;

    using Dasync.Collections;

    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;

    using Serilog;

    using StoneAssemblies.Data.Services;
    using StoneAssemblies.Hosting.Extensions;
    using StoneAssemblies.MassAuth.Messages;
    using StoneAssemblies.MassAuth.Rules.Interfaces;
    using StoneAssemblies.MassAuth.Rules.SqlClient.Rules;
    using StoneAssemblies.MassAuth.Rules.SqlClient.Services;
    using StoneAssemblies.MassAuth.Rules.SqlClient.Services.Interfaces;

    /// <summary>
    ///     The service collection extensions.
    /// </summary>
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        ///     The registered stored procedure rules.
        /// </summary>
        private static readonly ConcurrentDictionary<IServiceCollection, HashSet<string>> RegisteredStoredProcedureRulesPerServiceCollection = new ConcurrentDictionary<IServiceCollection, HashSet<string>>();

        /// <summary>
        ///     Register SQL Client stored procedure based rules from configuration.
        /// </summary>
        /// <param name="serviceCollection">
        ///     The service collection.
        /// </param>
        /// <param name="configuration">
        ///     The configuration.
        /// </param>
        public static void RegisterSqlClientStoredProcedureBasedRulesFromConfiguration(
            this IServiceCollection serviceCollection, IConfiguration configuration)
        {
            var configurationSection = configuration?.GetSection("SqlClientStoredProcedureBasedRules");
            if (configurationSection == null)
            {
                Log.Warning("Expected configuration section 'SqlClientStoredProcedureBasedRules' not found");
                return;
            }

            var patterns = new List<string>();
            configurationSection?.GetSection("Patterns")?.Bind(patterns);

            var connectionStrings = new List<string>();
            configurationSection?.GetSection("ConnectionStrings")?.Bind(connectionStrings);
            if (connectionStrings.Count == 0)
            {
                Log.Warning("No connection strings are specified in 'ConnectionStrings' section for 'SqlClientStoredProcedureBasedRules'");
                return;
            }

            var connectionFactory = new ConnectionFactory();
            serviceCollection.AddSingleton(connectionFactory);
            foreach (var connectionString in connectionStrings)
            {
                var sqlServerDatabaseInspector = new SqlClientDatabaseInspector(connectionFactory, connectionString);
                serviceCollection.RegisterStoredProcedureBasedRules(sqlServerDatabaseInspector, patterns);
            }
        }

        /// <summary>
        ///     Register stored procedure based rules.
        /// </summary>
        /// <param name="serviceCollection">
        ///     The service collection.
        /// </param>
        /// <param name="databaseInspector">
        ///     The database inspector.
        /// </param>
        /// <param name="patterns">
        ///     The name patterns.
        /// </param>
        public static void RegisterStoredProcedureBasedRules(
            this IServiceCollection serviceCollection, IDatabaseInspector databaseInspector, IEnumerable<string> patterns)
        {
            serviceCollection.RegisterStoredProcedureBasedRulesUsingMappings(databaseInspector);
            serviceCollection.RegisterStoredProcedureBasedRulesUsingPatterns(databaseInspector, patterns);
        }

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
        /// <param name="priority">
        ///     The priority.
        /// </param>
        private static void RegisterStoredProcedureBasedRule(
            this IServiceCollection serviceCollection,
            Type messageType,
            string ruleName,
            string connectionString,
            string storedProcedureName,
            int priority = 0)
        {
            Log.Information(
                "Registering stored procedure based rule '{RuleName}' for message type '{MessageTypeName}' using stored procedure '{StoredProcedureName}' with priority {Priority}",
                ruleName,
                messageType.Name,
                storedProcedureName,
                priority);

            var registeredStoredProcedures = RegisteredStoredProcedureRulesPerServiceCollection.GetOrAdd(
                serviceCollection,
                collection => new HashSet<string>());
            lock (registeredStoredProcedures)
            {
                var key = $"{connectionString}-{storedProcedureName}-{messageType.Name}";
                if (!registeredStoredProcedures.Contains(key))
                {
                    var sqlClientConnectionFactory = serviceCollection.GetRegisteredInstance<ConnectionFactory>();
                    var authorizationRequestMessageType = typeof(AuthorizationRequestMessage<>).MakeGenericType(messageType);
                    var ruleInterfaceType = typeof(IRule<>).MakeGenericType(authorizationRequestMessageType);
                    var ruleType = typeof(SqlClientStoredProcedureBasedRule<>).MakeGenericType(authorizationRequestMessageType);
                    serviceCollection.AddSingleton(
                        ruleInterfaceType,
                        sp => Activator.CreateInstance(
                            ruleType,
                            sqlClientConnectionFactory,
                            ruleName,
                            messageType,
                            connectionString,
                            storedProcedureName,
                            priority));

                    registeredStoredProcedures.Add(key);
                }
            }
        }

        /// <summary>
        ///     Register stored procedure based rules using mappings.
        /// </summary>
        /// <param name="serviceCollection">
        ///     The service collection.
        /// </param>
        /// <param name="databaseInspector">
        ///     The database inspector.
        /// </param>
        private static void RegisterStoredProcedureBasedRulesUsingMappings(
            this IServiceCollection serviceCollection, IDatabaseInspector databaseInspector)
        {
            Log.Information("Registering stored procedure based rules using mappings");

            var mappings = databaseInspector.GetMappings().ToList();
            if (mappings.Count == 0)
            {
                Log.Warning("No mappings found for stored procedure based rules");
                return;
            }

            foreach (var mapping in mappings)
            {
                var messageTypeName = mapping.MessageTypeName;
                var ruleName = mapping.RuleName;
                var storedProcedureName = mapping.StoredProcedure;
                var messageType = MessageTypeHelper.GetMessageType(messageTypeName);
                if (messageType != null)
                {
                    serviceCollection.RegisterStoredProcedureBasedRule(
                        messageType,
                        ruleName,
                        databaseInspector.ConnectionString,
                        storedProcedureName,
                        mapping.Priority);
                }
            }
        }

        /// <summary>
        ///     Registers stored procedure based rules using patterns.
        /// </summary>
        /// <param name="serviceCollection">
        ///     The service collection.
        /// </param>
        /// <param name="databaseInspector">
        ///     The database inspector.
        /// </param>
        /// <param name="patterns">
        ///     The patterns.
        /// </param>
        private static void RegisterStoredProcedureBasedRulesUsingPatterns(
            this IServiceCollection serviceCollection, IDatabaseInspector databaseInspector, IEnumerable<string> patterns)
        {
            Log.Information("Registering stored procedure based rules using patterns");

            const string MessageTypeCapturingGroupName = "messageType";
            const string RuleNameCapturingGroupName = "ruleName";
            var regexOptions = RegexOptions.IgnoreCase | RegexOptions.Multiline | RegexOptions.Compiled;
            var regularExpressions = patterns.Select(pattern => new Regex(pattern, regexOptions)).ToList();
            if (regularExpressions.Count == 0)
            {
                Log.Warning("No patterns specified for stored procedure based rules");

                return;
            }

            var storedProcedures = databaseInspector.GetStoredProcedures().Distinct().ToList();
            foreach (var storedProcedureName in storedProcedures)
            {
                var match = regularExpressions.Select(r => r.Match(storedProcedureName)).FirstOrDefault(m => m.Success);
                if (match != null && match.Groups.ContainsKey(MessageTypeCapturingGroupName))
                {
                    var messageTypeName = match.Groups[MessageTypeCapturingGroupName].Value;
                    var ruleName = string.Empty;
                    if (match.Groups.ContainsKey(RuleNameCapturingGroupName))
                    {
                        ruleName = match.Groups[RuleNameCapturingGroupName].Value;
                    }

                    var messageType = MessageTypeHelper.GetMessageType(messageTypeName);
                    if (messageType != null)
                    {
                        serviceCollection.RegisterStoredProcedureBasedRule(
                            messageType,
                            ruleName,
                            databaseInspector.ConnectionString,
                            storedProcedureName);
                    }
                }
            }
        }
    }
}