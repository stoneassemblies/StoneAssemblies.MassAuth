namespace StoneAssemblies.MassAuth.Rules.SqlClient.Services
{
    using System.Collections.Generic;
    using System.Data;

    using Microsoft.Data.SqlClient;

    using StoneAssemblies.MassAuth.Rules.SqlClient.Services.Interfaces;

    /// <summary>
    ///     The SqlServerDatabaseInspector.
    /// </summary>
    public class SqlClientDatabaseInspector : IDatabaseInspector
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="SqlClientDatabaseInspector" /> class.
        /// </summary>
        public SqlClientDatabaseInspector(string connectionString)
        {
            this.ConnectionString = connectionString;
        }

        /// <summary>
        /// The connection string.
        /// </summary>
        public string ConnectionString { get; }

        /// <inheritdoc />
        public IEnumerable<string> GetStoredProcedures()
        {
            using var sqlConnection = new SqlConnection(this.ConnectionString);
            var sqlCommand = sqlConnection.CreateCommand();
            sqlCommand.CommandText = "SELECT * FROM INFORMATION_SCHEMA.ROUTINES WHERE ROUTINE_TYPE = 'PROCEDURE'";
            try
            {
                sqlConnection.Open();
                var sqlDataReader = sqlCommand.ExecuteReader();
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