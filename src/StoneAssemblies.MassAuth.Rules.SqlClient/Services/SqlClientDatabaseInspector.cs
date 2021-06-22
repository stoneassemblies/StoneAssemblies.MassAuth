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

    using Microsoft.Data.SqlClient;

    using Serilog;

    using StoneAssemblies.MassAuth.Rules.SqlClient.Models;
    using StoneAssemblies.MassAuth.Rules.SqlClient.Services.Interfaces;

    /// <summary>
    ///     The SqlServerDatabaseInspector.
    /// </summary>
    public class SqlClientDatabaseInspector : IDatabaseInspector
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="SqlClientDatabaseInspector" /> class.
        /// </summary>
        /// <param name="connectionString">
        ///     The connection string.
        /// </param>
        public SqlClientDatabaseInspector(string connectionString)
        {
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
                using var sqlConnection = new SqlConnection(this.ConnectionString);
                var sqlCommand = sqlConnection.CreateCommand();
                sqlCommand.CommandText =
                    $"SELECT [RuleName], [MessageTypeName], [StoredProcedure], [Priority] FROM [dbo].[Mappings] WHERE [StoredProcedure]='{storedProcedure}'";

                try
                {
                    sqlConnection.Open();
                }
                catch (Exception ex)
                {
                    Log.Error(ex, "Error opening connection to read information schema routines");
                }

                try
                {
                    if (sqlConnection.State == ConnectionState.Open)
                    {
                        SqlDataReader sqlDataReader = null;
                        try
                        {
                            sqlDataReader = sqlCommand.ExecuteReader();
                        }
                        catch (Exception ex)
                        {
                            Log.Warning(ex, "Error reading rules stored procedures message type mappings");
                        }

                        if (sqlDataReader == null)
                        {
                            yield break;
                        }

                        while (sqlDataReader.Read())
                        {
                            yield return new Mapping(
                                sqlDataReader.GetString(0),
                                sqlDataReader.GetString(1),
                                sqlDataReader.GetString(2),
                                sqlDataReader.GetInt32(3)
                                );
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

        /// <inheritdoc />
        public IEnumerable<string> GetStoredProcedures()
        {
            using var sqlConnection = new SqlConnection(this.ConnectionString);
            var sqlCommand = sqlConnection.CreateCommand();
            sqlCommand.CommandText = "SELECT * FROM INFORMATION_SCHEMA.ROUTINES WHERE ROUTINE_TYPE = 'PROCEDURE'";
            try
            {
                sqlConnection.Open();
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error opening connection to read information schema routines");
            }

            if (sqlConnection.State == ConnectionState.Open)
            {
                try
                {
                    SqlDataReader sqlDataReader = null;
                    try
                    {
                        sqlDataReader = sqlCommand.ExecuteReader();
                    }
                    catch (Exception ex)
                    {
                        Log.Error(ex, "Error reading information schema routines");
                    }

                    if (sqlDataReader == null)
                    {
                        yield break;
                    }

                    while (sqlDataReader.Read())
                    {
                        var storeProcedureName = sqlDataReader.GetString(2);
                        yield return storeProcedureName;
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