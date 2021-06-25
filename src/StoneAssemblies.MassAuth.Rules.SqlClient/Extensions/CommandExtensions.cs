// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CommandExtensions.cs" company="Stone Assemblies">
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

    using Serilog;

    /// <summary>
    ///     The command extensions.
    /// </summary>
    public static class CommandExtensions
    {
        /// <summary>
        ///     Execute reader safety.
        /// </summary>
        /// <param name="command">
        ///     The command.
        /// </param>
        /// <returns>
        ///     The <see cref="IDataAdapter" />.
        /// </returns>
        public static IDataReader ExecuteReaderSafety(this IDbCommand command)
        {
            IDataReader dataReader = null;
            try
            {
                dataReader = command.ExecuteReader();
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error executing reader");
            }

            return dataReader;
        }

        /// <summary>
        ///     This is the asynchronous version of ExecuteScalar(). Providers should override with an appropriate implementation.
        ///     The cancellation token may optionally be ignored. The default implementation invokes the synchronous
        ///     ExecuteScalar() method and returns a completed task, blocking the calling thread. The default implementation will
        ///     return a cancelled task if passed an already cancelled cancellation token. Exceptions thrown by ExecuteScalar will
        ///     be communicated via the returned Task Exception property. Do not invoke other methods and properties of the
        ///     DbCommand object until the returned Task is complete.
        /// </summary>
        /// <param name="command">
        ///     The command.
        /// </param>
        /// <param name="cancellationToken">
        ///     The cancellation token.
        /// </param>
        /// <returns>
        ///     The <see cref="Task" />.
        /// </returns>
        public static async Task<object?> ExecuteScalarAsync(this IDbCommand command, CancellationToken cancellationToken = default)
        {
            if (command is DbCommand dbConnection)
            {
                return await dbConnection.ExecuteScalarAsync(cancellationToken);
            }

            return command.ExecuteScalar();
        }
    }
}