// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ServiceCollectionExtensions.cs" company="Stone Assemblies">
// Copyright © 2021 - 2021 Stone Assemblies. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace StoneAssemblies.MassAuth.Rules.SqlClient.Extensions
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text.RegularExpressions;

    using Microsoft.Extensions.DependencyInjection;

    using StoneAssemblies.MassAuth.Messages;
    using StoneAssemblies.MassAuth.Rules.Interfaces;
    using StoneAssemblies.MassAuth.Rules.SqlClient.Rules;
    using StoneAssemblies.MassAuth.Rules.SqlClient.Services.Interfaces;

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
            var ruleType = typeof(SqlClientStoredProcedureBasedRule<>).MakeGenericType(authorizationRequestMessageType);
            serviceCollection.AddSingleton(
                ruleInterfaceType,
                sp => Activator.CreateInstance(ruleType, ruleName, messageType, connectionString, storedProcedureName));
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
            this IServiceCollection serviceCollection,
            IDatabaseInspector databaseInspector,
            IEnumerable<string> patterns)
        {
            const string MessageTypeCapturingGroupName = "messageType";
            const string RuleNameCapturingGroupName = "ruleName";

            var regexOptions = RegexOptions.IgnoreCase | RegexOptions.Multiline | RegexOptions.Compiled;
            var regularExpressions = patterns.Select(pattern => new Regex(pattern, regexOptions)).ToList();
            if (regularExpressions.Count == 0)
            {
                return;
            }

            foreach (var storedProcedureName in databaseInspector.GetStoredProcedures().Distinct())
            {
                var match = regularExpressions.Select(r => r.Match(storedProcedureName)).FirstOrDefault(m => m.Success);

                if (match != null && match.Groups.ContainsKey(MessageTypeCapturingGroupName))
                {
                    var messageTypeName = match.Groups[MessageTypeCapturingGroupName].Value;
                    var messageType = AppDomain.CurrentDomain.GetAssemblies()
                        .SelectMany(assembly => assembly.GetTypes()).FirstOrDefault(
                            type => typeof(MessageBase).IsAssignableFrom(type) && type.Name == messageTypeName);

                    var ruleName = string.Empty;
                    if (match.Groups.ContainsKey(RuleNameCapturingGroupName))
                    {
                        ruleName = match.Groups[RuleNameCapturingGroupName].Value;
                    }

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