﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SqlClientStoredProcedureBasedRule.cs" company="Stone Assemblies">
// Copyright © 2021 - 2021 Stone Assemblies. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace StoneAssemblies.MassAuth.Rules.SqlClient.Rules
{
    using System;
    using System.Data;
    using System.Threading.Tasks;

    using Newtonsoft.Json;

    using StoneAssemblies.MassAuth.Rules.Interfaces;
    using StoneAssemblies.MassAuth.Rules.SqlClient.Extensions;
    using StoneAssemblies.MassAuth.Rules.SqlClient.Services.Interfaces;

    /// <summary>
    ///     The store procedure based rule.
    /// </summary>
    /// <typeparam name="TMessage">
    ///     The message type.
    /// </typeparam>
    public sealed class SqlClientStoredProcedureBasedRule<TMessage> : IRule<TMessage>
    {
        /// <summary>
        ///     The connection factory.
        /// </summary>
        private readonly IConnectionFactory connectionFactory;

        /// <summary>
        ///     The connection string.
        /// </summary>
        private readonly string connectionString;

        /// <summary>
        ///     The message type.
        /// </summary>
        private readonly Type messageType;

        /// <summary>
        ///     The rule name.
        /// </summary>
        private readonly string ruleName;

        /// <summary>
        ///     The store procedure name.
        /// </summary>
        private readonly string storedProcedureName;

        /// <summary>
        ///     Initializes a new instance of the <see cref="SqlClientStoredProcedureBasedRule{TMessage}" /> class.
        /// </summary>
        /// <param name="connectionFactory">
        ///     The connection factory.
        /// </param>
        /// <param name="ruleName">
        ///     The rule name.
        /// </param>
        /// <param name="messageType">
        ///     The message type.
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
        public SqlClientStoredProcedureBasedRule(
            IConnectionFactory connectionFactory, string ruleName, Type messageType, string connectionString,
            string storedProcedureName, int priority)
        {
            this.connectionFactory = connectionFactory;
            this.ruleName = ruleName;
            this.messageType = messageType;
            this.connectionString = connectionString;
            this.storedProcedureName = storedProcedureName;
            this.Priority = priority;
        }

        /// <inheritdoc />
        public bool IsEnabled => true;

        /// <inheritdoc />
        public string Name =>
            $"{this.ruleName} for '{this.messageType.Name}' message type based on the '{this.storedProcedureName}' stored procedure"
                .TrimStart();

        /// <inheritdoc />
        public int Priority { get; }

        /// <summary>
        ///     Evaluate the rules by running the stored procedure.
        /// </summary>
        /// <param name="message">
        ///     The messages.
        /// </param>
        /// <returns>
        ///     <c>true</c> if the evaluation succeeds, otherwise <c>false</c>.
        /// </returns>
        public async Task<bool> EvaluateAsync(TMessage message)
        {
            var connection = this.connectionFactory.Create(this.connectionString);

            var command = connection.CreateCommand();
            command.CommandType = CommandType.StoredProcedure;
            command.CommandText = this.storedProcedureName;

            var messageParameter = command.CreateParameter();
            messageParameter.ParameterName = "@message";
            messageParameter.DbType = DbType.String;
            messageParameter.Value = JsonConvert.SerializeObject(message);

            command.Parameters.Add(messageParameter);

            try
            {
                await connection.OpenAsync();
                var value = await command.ExecuteScalarAsync();
                if (value != null)
                {
                    return Convert.ToBoolean(value);
                }
            }
            finally
            {
                if (connection.State == ConnectionState.Open)
                {
                    await connection.CloseAsync();
                }

                await connection.DisposeAsync();
            }

            return false;
        }
    }
}