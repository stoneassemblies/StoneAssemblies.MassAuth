// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SqlClientConnectionFactory.cs" company="Stone Assemblies">
// Copyright © 2021 - 2021 Stone Assemblies. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace StoneAssemblies.MassAuth.Rules.SqlClient.Services
{
    using System.Data;

    using Microsoft.Data.SqlClient;

    using StoneAssemblies.MassAuth.Rules.SqlClient.Services.Interfaces;

    /// <summary>
    ///     The SQL Client Connection factory.
    /// </summary>
    public class SqlClientConnectionFactory : IConnectionFactory
    {
        /// <summary>
        ///     Creates a connection.
        /// </summary>
        /// <param name="connectionString">
        ///     The connection string.
        /// </param>
        /// <returns>
        ///     The <see cref="IDbConnection" />.
        /// </returns>
        public IDbConnection Create(string connectionString)
        {
            return new SqlConnection(connectionString);
        }
    }
}