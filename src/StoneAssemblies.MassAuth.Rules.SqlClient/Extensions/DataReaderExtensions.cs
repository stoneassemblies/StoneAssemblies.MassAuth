// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DataReaderExtensions.cs" company="Stone Assemblies">
// Copyright © 2021 - 2021 Stone Assemblies. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace StoneAssemblies.MassAuth.Rules.SqlClient.Extensions
{
    using System;
    using System.Collections.Generic;
    using System.Data;

    using Serilog;

    /// <summary>
    ///     The data reader extensions.
    /// </summary>
    public static class DataReaderExtensions
    {
        /// <summary>
        ///     Selects records from data reader.
        /// </summary>
        /// <param name="dataReader">
        ///     The data reader.
        /// </param>
        /// <param name="projection">
        ///     The projection.
        /// </param>
        /// <typeparam name="T">
        ///     The result type.
        /// </typeparam>
        /// <returns>
        ///     The <see cref="IEnumerable{T}" />.
        /// </returns>
        public static IEnumerable<T> Select<T>(this IDataReader dataReader, Func<IDataReader, T> projection)
        {
            while (dataReader.ReadSafety())
            {
                T value;

                try
                {
                    value = projection(dataReader);
                }
                catch (Exception ex)
                {
                    Log.Error(ex, "Error reading data from reader");

                    yield break;
                }

                yield return value;
            }
        }

        /// <summary>
        ///     Read safety.
        /// </summary>
        /// <param name="dataReader">
        ///     The data reader.
        /// </param>
        /// <returns>
        ///     The <see cref="bool" />.
        /// </returns>
        public static bool ReadSafety(this IDataReader dataReader)
        {
            try
            {
                return dataReader.Read();
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error reading the next record from data reader");

                return false;
            }
        }
    }
}