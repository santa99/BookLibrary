using System.Data.Common;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Options;

namespace Attendance.DataAccess.Utilities
{
    public interface IDatabaseConnectionProvider
    {
        DbConnection DbConnection();
    }

    public sealed class DatabaseConnectionProvider : IDatabaseConnectionProvider
    {
        private readonly DatabaseConnections _databaseSettings;

        public DatabaseConnectionProvider(IOptions<DatabaseConnections> databaseSettings)
        {
            _databaseSettings = databaseSettings.Value;
        }

        public DbConnection DbConnection()
        {
            return new SqlConnection(_databaseSettings.Attendance);
        }
    }
}