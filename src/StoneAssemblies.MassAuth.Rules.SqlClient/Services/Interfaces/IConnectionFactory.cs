// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IConnectionFactory.cs" company="Stone Assemblies">
// Copyright © 2021 - 2021 Stone Assemblies. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace StoneAssemblies.MassAuth.Rules.SqlClient.Services.Interfaces
{
    using System.Data;

    /// <summary>
    ///     The ConnectionFactory interface.
    /// </summary>
    public interface IConnectionFactory
    {
        /// <summary>
        ///     Creates a database connection.
        /// </summary>
        /// <param name="connectionString">
        ///     The connection string.
        /// </param>
        /// <returns>
        ///     The <see cref="IDbConnection" />.
        /// </returns>
        IDbConnection Create(string connectionString);
    }
}