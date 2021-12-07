// --------------------------------------------------------------------------------------------------------------------
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

    using Serilog;

    using StoneAssemblies.Data.Extensions;
    using StoneAssemblies.Data.Services.Interfaces;
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
        public SqlClientStoredProcedureBasedRule(IConnectionFactory connectionFactory, string ruleName, Type messageType, string connectionString, string storedProcedureName, int priority)
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
        public async Task<EvaluationResult> EvaluateAsync(TMessage message)
        {
            var evaluationResult = EvaluationResult.Error();

            var connection = this.connectionFactory.Create(this.connectionString);

            var command = connection.CreateCommand();
            command.CommandType = CommandType.StoredProcedure;
            command.CommandText = this.storedProcedureName;

            command.AddParameterWithValue("@message", JsonConvert.SerializeObject(message));

            try
            {
                await connection.OpenAsync();
                var reader = await command.ExecuteReaderAsync();
                if (await reader.ReadAsync() && reader.FieldCount > 0)
                {
                    var succeeded = false;

                    if (reader.GetFieldType(0) == typeof(bool))
                    {
                        succeeded = reader.GetBoolean(0);
                    }
                    else if (reader.GetFieldType(0) == typeof(short))
                    {
                        succeeded = Convert.ToBoolean(reader.GetInt16(0));
                    }
                    else if (reader.GetFieldType(0) == typeof(int))
                    {
                        succeeded = Convert.ToBoolean(reader.GetInt32(0));
                    }
                    else if (reader.GetFieldType(0) == typeof(long))
                    {
                        succeeded = Convert.ToBoolean(reader.GetInt64(0));
                    }
                    else
                    {
                        Log.Warning(
                            "Can't read the result of rule '{RuleName}'. The value type must be Boolean or Integer",
                            this.ruleName);
                    }

                    if (succeeded)
                    {
                        evaluationResult = EvaluationResult.Success();
                    }
                    else
                    {
                        if (reader.FieldCount > 1 && reader.GetFieldType(1) == typeof(string))
                        {
                            evaluationResult = EvaluationResult.Error(reader.GetString(1));
                        }
                        else
                        {
                            Log.Warning(
                                "Can't read the error reason of rule '{RuleName}', the value type must be string",
                                this.ruleName);
                        }
                    }
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

            return evaluationResult;
        }
    }
}