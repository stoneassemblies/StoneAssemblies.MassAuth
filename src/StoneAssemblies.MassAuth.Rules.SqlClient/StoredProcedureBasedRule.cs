// --------------------------------------------------------------------------------------------------------------------
// <copyright file="StoredProcedureBasedRule.cs" company="Stone Assemblies">
// Copyright � 2021 - 2021 Stone Assemblies. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace StoneAssemblies.MassAuth.Rules.SqlClient
{
    using System;
    using System.Data;
    using System.Threading.Tasks;

    using Microsoft.Data.SqlClient;

    using Newtonsoft.Json;

    using StoneAssemblies.MassAuth.Rules.Interfaces;

    /// <summary>
    ///     The store procedure based rule.
    /// </summary>
    /// <typeparam name="TMessage">
    ///     The message type.
    /// </typeparam>
    public sealed class StoredProcedureBasedRule<TMessage> : IRule<TMessage>
    {
        /// <summary>
        ///     The connection string.
        /// </summary>
        private readonly string connectionString;

        /// <summary>
        ///     The store procedure name.
        /// </summary>
        private readonly string storeProcedureName;

        /// <summary>
        ///     Initializes a new instance of the <see cref="StoredProcedureBasedRule{TMessage}" /> class.
        /// </summary>
        /// <param name="connectionString">
        ///     The connection string.
        /// </param>
        /// <param name="storeProcedureName">
        ///     The store procedure name.
        /// </param>
        public StoredProcedureBasedRule(string connectionString, string storeProcedureName)
        {
            this.connectionString = connectionString;
            this.storeProcedureName = storeProcedureName;
        }

        /// <inheritdoc />
        public bool IsEnabled => true;

        /// <inheritdoc />
        public string Name => $"Store procedure '{this.storeProcedureName}'";

        /// <inheritdoc />
        public int Priority => 0;

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
            await using var connection = new SqlConnection(this.connectionString);
            var sqlCommand = connection.CreateCommand();
            sqlCommand.CommandType = CommandType.StoredProcedure;
            sqlCommand.CommandText = this.storeProcedureName;
            var serializedMessage = JsonConvert.SerializeObject(message);
            sqlCommand.Parameters.AddWithValue("@message", serializedMessage);

            try
            {
                await connection.OpenAsync();
                var value = await sqlCommand.ExecuteScalarAsync();
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
            }

            return false;
        }
    }
}