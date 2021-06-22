namespace StoneAssemblies.MassAuth.Tests.Rules.SqlClient
{
    using System.Linq;

    using StoneAssemblies.MassAuth.Rules.SqlClient.Services;

    using Xunit;

    public class SqlClientDatabaseInspectorFacts
    {
        public class The_GetMappings_Method
        {
            [Fact]
            public void Does_Not_Throw_Exception_With_Invalid_ConnectionStrings()
            {
                var connectionString = "Server=localhost,321;Database=DB;User Id=User;Password=Password;MultipleActiveResultSets=true;Connection Timeout=30";
                var sqlClientDatabaseInspector = new SqlClientDatabaseInspector(connectionString);
                var mappings = sqlClientDatabaseInspector.GetMappings().ToList();
                Assert.Empty(mappings);
            }
        }

        public class The_GetStoredProcedures_Method
        {
            [Fact]
            public void Does_Not_Throw_Exception_With_Invalid_ConnectionStrings()
            {
                var connectionString = "Server=localhost,321;Database=DB;User Id=User;Password=Password;MultipleActiveResultSets=true;Connection Timeout=30";
                var sqlClientDatabaseInspector = new SqlClientDatabaseInspector(connectionString);
                var storedProcedures = sqlClientDatabaseInspector.GetStoredProcedures().ToList();
                Assert.Empty(storedProcedures);
            }
        }
    }
}