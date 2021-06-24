// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CommandExtensions.cs" company="Stone Assemblies">
// Copyright © 2021 - 2021 Stone Assemblies. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace StoneAssemblies.MassAuth.Rules.SqlClient.Extensions
{
    using System;
    using System.Data;

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
    }
}