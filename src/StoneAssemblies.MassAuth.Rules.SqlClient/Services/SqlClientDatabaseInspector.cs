// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SqlClientDatabaseInspector.cs" company="Stone Assemblies">
// Copyright © 2021 - 2021 Stone Assemblies. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace StoneAssemblies.MassAuth.Rules.SqlClient.Services
{
    using System;
    using System.Collections.Generic;
    using System.Data;

    using Serilog;

    using StoneAssemblies.Data.Extensions;
    using StoneAssemblies.Data.Services.Interfaces;
    using StoneAssemblies.MassAuth.Rules.SqlClient.Extensions;
    using StoneAssemblies.MassAuth.Rules.SqlClient.Models;
    using StoneAssemblies.MassAuth.Rules.SqlClient.Services.Interfaces;

    /// <summary>
    ///     The SqlServerDatabaseInspector.
    /// </summary>
    public class SqlClientDatabaseInspector : IDatabaseInspector
    {
        /// <summary>
        /// The database connection factory.
        /// </summary>
        private readonly IDbConnectionFactory connectionFactory;

        /// <summary>
        ///     Initializes a new instance of the <see cref="SqlClientDatabaseInspector" /> class.
        /// </summary>
        /// <param name="connectionFactory">
        ///     The connection factory.
        /// </param>
        /// <param name="connectionString">
        ///     The connection string.
        /// </param>
        public SqlClientDatabaseInspector(IDbConnectionFactory connectionFactory, string connectionString)
        {
            this.connectionFactory = connectionFactory;
            this.ConnectionString = connectionString;
        }

        /// <inheritdoc />
        public string ConnectionString { get; }

        /// <inheritdoc />
        public IEnumerable<Mapping> GetMappings()
        {
            var storedProcedures = this.GetStoredProcedures();
            foreach (var storedProcedure in storedProcedures)
            {
                using var connection = this.connectionFactory.Create(this.ConnectionString);
                var command = connection.CreateCommand();
                command.CommandText =
                    $"SELECT [RuleName], [MessageTypeName], [StoredProcedure], [Priority] FROM [dbo].[Mappings] WHERE [StoredProcedure]='{storedProcedure}'";

                try
                {
                    connection.Open();
                }
                catch (Exception ex)
                {
                    Log.Error(ex, "Error opening connection to read information schema routines");
                }

                try
                {
                    if (connection.State == ConnectionState.Open)
                    {
                        var dataReader = command.ExecuteReader(true);
                        if (dataReader == null)
                        {
                            Log.Warning("No data reader for mappings");

                            yield break;
                        }

                        var mappings = dataReader.GetAll(
                            reader => new Mapping(
                                dataReader.GetString(0),
                                dataReader.GetString(1),
                                dataReader.GetString(2),
                                dataReader.GetInt32(3)),
                            true);

                        foreach (var mapping in mappings)
                        {
                            yield return mapping;
                        }
                    }
                }
                finally
                {
                    if (connection.State == ConnectionState.Open)
                    {
                        connection.Close();
                    }
                }
            }
        }

        /// <inheritdoc />
        public IEnumerable<string> GetStoredProcedures()
        {
            using var connection = this.connectionFactory.Create(this.ConnectionString);
            var command = connection.CreateCommand();
            command.CommandText = "SELECT * FROM INFORMATION_SCHEMA.ROUTINES WHERE ROUTINE_TYPE = 'PROCEDURE'";

            try
            {
                connection.Open();
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error opening connection to read information schema routines");
            }

            if (connection.State == ConnectionState.Open)
            {
                try
                {
                    var dataReader = command.ExecuteReader(true);
                    if (dataReader == null)
                    {
                        yield break;
                    }

                    foreach (var storedProcedure in dataReader.GetAll(reader => reader.GetString(2), true))
                    {
                        yield return storedProcedure;
                    }
                }
                finally
                {
                    if (connection.State == ConnectionState.Open)
                    {
                        connection.Close();
                    }
                }
            }
        }
    }
}