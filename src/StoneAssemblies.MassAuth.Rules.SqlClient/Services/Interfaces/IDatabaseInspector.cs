// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IDatabaseInspector.cs" company="Stone Assemblies">
// Copyright © 2021 - 2021 Stone Assemblies. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace StoneAssemblies.MassAuth.Rules.SqlClient.Services.Interfaces
{
    using System.Collections.Generic;

    /// <summary>
    /// The DatabaseInspector interface.
    /// </summary>
    public interface IDatabaseInspector
    {
        /// <summary>
        /// Gets the connection string.
        /// </summary>
        string ConnectionString { get; }

        /// <summary>
        /// Gets the stored procedures.
        /// </summary>
        /// <returns>
        /// The <see cref="IEnumerable{String}"/>.
        /// </returns>
        IEnumerable<string> GetStoredProcedures();
    }
}
