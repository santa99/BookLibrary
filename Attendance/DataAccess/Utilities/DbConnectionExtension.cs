using System.Data;
using System.Data.Common;
using Dapper;

namespace Attendance.DataAccess.Utilities
{
    public static class DbConnectionExtension
    {
        public static async Task<IEnumerable<T>> QueryAsync<T>(this DbConnection dbConnection,
            string storedProcedureName,
            DynamicParameters dynamicParameters, CancellationToken cancellationToken)
        {
            var commandDefinition = new CommandDefinition(storedProcedureName, dynamicParameters,
                commandType: CommandType.StoredProcedure,
                cancellationToken: cancellationToken);
            return await dbConnection.QueryAsync<T>(commandDefinition);
        }

        public static async Task<int> ExecuteAsync(this DbConnection dbConnection, string storedProcedureName,
            DynamicParameters dynamicParameters,
            CancellationToken cancellationToken, int? commandTimeout = null)
        {
            var commandDefinition = new CommandDefinition(storedProcedureName, dynamicParameters,
                commandType: CommandType.StoredProcedure,
                commandTimeout: commandTimeout, cancellationToken: cancellationToken);

            return await dbConnection.ExecuteAsync(commandDefinition);
        }
    }
}