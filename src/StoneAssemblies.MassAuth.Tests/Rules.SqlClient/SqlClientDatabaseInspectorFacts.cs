// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SqlClientDatabaseInspectorFacts.cs" company="Stone Assemblies">
// Copyright © 2021 - 2021 Stone Assemblies. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace StoneAssemblies.MassAuth.Tests.Rules.SqlClient
{
    using System.Linq;

    using StoneAssemblies.MassAuth.Rules.SqlClient.Services;

    using Xunit;

    /// <summary>
    ///     The sql client database inspector facts.
    /// </summary>
    public class SqlClientDatabaseInspectorFacts
    {
        /// <summary>
        ///     The GetMappings method.
        /// </summary>
        public class The_GetMappings_Method
        {
            /// <summary>
            ///     Does not throw exception with invalid connection strings.
            /// </summary>
            [Fact]
            public void Does_Not_Throw_Exception_With_Invalid_ConnectionStrings()
            {
                var connectionString =
                    "Server=localhost,321;Database=DB;User Id=User;Password=Password;MultipleActiveResultSets=true;Connection Timeout=30";
                var sqlClientDatabaseInspector = new SqlClientDatabaseInspector(connectionString);
                var mappings = sqlClientDatabaseInspector.GetMappings().ToList();
                Assert.Empty(mappings);
            }
        }

        /// <summary>
        ///     The GetStoredProcedures method.
        /// </summary>
        public class The_GetStoredProcedures_Method
        {
            /// <summary>
            ///     Does not throw exception with invalid connection strings.
            /// </summary>
            [Fact]
            public void Does_Not_Throw_Exception_With_Invalid_ConnectionStrings()
            {
                var connectionString =
                    "Server=localhost,321;Database=DB;User Id=User;Password=Password;MultipleActiveResultSets=true;Connection Timeout=30";
                var sqlClientDatabaseInspector = new SqlClientDatabaseInspector(connectionString);
                var storedProcedures = sqlClientDatabaseInspector.GetStoredProcedures().ToList();
                Assert.Empty(storedProcedures);
            }
        }
    }
}