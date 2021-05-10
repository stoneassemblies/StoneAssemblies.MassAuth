// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ServiceCollectionExtensions.cs" company="Stone Assemblies">
// Copyright © 2021 - 2021 Stone Assemblies. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace StoneAssemblies.MassAuth.Rules.SqlClient.Extensions
{
    using System;

    using Microsoft.Extensions.DependencyInjection;

    using StoneAssemblies.MassAuth.Messages;
    using StoneAssemblies.MassAuth.Rules.Interfaces;

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
        /// <param name="connectionString">
        ///     The connection string.
        /// </param>
        /// <param name="storeProcedureName">
        ///     The store procedure name.
        /// </param>
        public static void RegisterStoredProcedureBasedRule(
            this IServiceCollection serviceCollection,
            Type messageType,
            string connectionString,
            string storeProcedureName)
        {
            var authorizationRequestMessageType = typeof(AuthorizationRequestMessage<>).MakeGenericType(messageType);
            var ruleInterfaceType = typeof(IRule<>).MakeGenericType(authorizationRequestMessageType);
            var ruleType = typeof(StoredProcedureBasedRule<>).MakeGenericType(authorizationRequestMessageType);
            serviceCollection.AddSingleton(
                ruleInterfaceType,
                sp => Activator.CreateInstance(ruleType, connectionString, storeProcedureName));
        }
    }
}