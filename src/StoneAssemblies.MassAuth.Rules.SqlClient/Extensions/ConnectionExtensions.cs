// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ConnectionExtensions.cs" company="Stone Assemblies">
// Copyright © 2021 - 2021 Stone Assemblies. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace StoneAssemblies.MassAuth.Rules.SqlClient.Extensions
{
    using System;
    using System.Data;
    using System.Data.Common;
    using System.Threading;
    using System.Threading.Tasks;

    /// <summary>
    ///     The connection extensions.
    /// </summary>
    public static class ConnectionExtensions
    {
        /// <summary>
        ///     The close async.
        /// </summary>
        /// <param name="connection">
        ///     The connection.
        /// </param>
        /// <returns>
        ///     The <see cref="Task" />.
        /// </returns>
        public static async Task CloseAsync(this IDbConnection connection)
        {
            if (connection is DbConnection dbConnection)
            {
                await dbConnection.CloseAsync();
            }
            else
            {
                connection.Close();
            }
        }

        /// <summary>
        ///     Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources
        ///     asynchronously.
        /// </summary>
        /// <param name="connection">
        ///     The connection.
        /// </param>
        /// <returns>
        ///     The <see cref="Task" />.
        /// </returns>
        public static async Task DisposeAsync(this IDbConnection connection)
        {
            if (connection is IAsyncDisposable dbConnection)
            {
                await dbConnection.DisposeAsync();
            }
            else
            {
                connection.Dispose();
            }
        }

        /// <summary>
        ///     This is the asynchronous version of Open(). Providers should override with an appropriate implementation. The
        ///     cancellation token can optionally be honored. The default implementation invokes the synchronous Open() call and
        ///     returns a completed task. The default implementation will return a cancelled task if passed an already cancelled
        ///     cancellationToken. Exceptions thrown by Open will be communicated via the returned Task Exception property. Do not
        ///     invoke other methods and properties of the DbConnection object until the returned Task is complete.
        /// </summary>
        /// <param name="connection">
        ///     The connection.
        /// </param>
        /// <param name="cancellationToken">
        ///     The cancellation token.
        /// </param>
        /// <returns>
        ///     The <see cref="Task" />.
        /// </returns>
        public static async Task OpenAsync(this IDbConnection connection, CancellationToken cancellationToken = default)
        {
            if (connection is DbConnection dbConnection)
            {
                await dbConnection.OpenAsync(cancellationToken);
            }
            else
            {
                connection.Open();
            }
        }
    }
}