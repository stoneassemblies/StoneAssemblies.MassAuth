namespace StoneAssemblies.MassAuth.Rules.SqlClient.Services.Interfaces
{
    using System.Collections.Generic;

    public interface IDatabaseInspector
    {
        string ConnectionString { get; }

        IEnumerable<string> GetStoredProcedures();
    }
}