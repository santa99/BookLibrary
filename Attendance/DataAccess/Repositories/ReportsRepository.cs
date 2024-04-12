using System.Data;
using Attendance.DataAccess.Dtos;
using Attendance.DataAccess.Utilities;
using Dapper;

namespace Attendance.DataAccess.Repositories
{
    public interface IReportsRepository
    {
        Task<long> InsertReport(
            int projectId, int employeeId, int positionId, int roleId, short timeLoggedInMinutes,
            CancellationToken cancellationToken);

        Task UpdateReport(
            long reportId, int employeeId, int positionId, int roleId, short timeLoggedInMinutes,
            CancellationToken cancellationToken);

        Task<List<ReportDto>> GetAllReportsAsync(CancellationToken cancellationToken);
        Task<List<ReportDto>> GetReportsByEmployeeIdAsync(int? employeeId, CancellationToken cancellationToken);
    }

    /*
    public class ReportsRepositoryMock : IReportsRepository
    {

        private Dictionary<long, Tuple<int, int, int, short>> _Reports =
            new Dictionary<long, Tuple<int, int, int, short>>();

        private Dictionary<long, string> _Projects = new Dictionary<long, string>();
        private Dictionary<long, string> _Employees = new Dictionary<long, string>();
        private Dictionary<long, Tuple<string, string, long>> _Role = new Dictionary<long, Tuple<string, string, long>>();
        
        
        public Task<long> InsertReport(int employeeId, int positionId, int roleId, short timeLoggedInMinutes,
            CancellationToken cancellationToken)
        {
            var nextKey = _Reports.Keys.Any() ? _Reports.Keys.Last() + 1 : 1; 
            
            _Reports.Add(nextKey, new Tuple<int, int, int, short>(employeeId, positionId, roleId, timeLoggedInMinutes));

            return Task.FromResult(nextKey);
        }

        public Task<long> InsertReport(int projectId, int employeeId, int positionId, int roleId, short timeLoggedInMinutes,
            CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task UpdateReport(long reportId, int employeeId, int positionId, int roleId, short timeLoggedInMinutes,
            CancellationToken cancellationToken)
        {
            if (_Reports.TryGetValue(reportId, out Tuple<int, int, int, short> value))
            {
                _Reports.Remove(reportId);
                _Reports.Add(reportId, new Tuple<int, int, int, short>(employeeId, positionId, roleId, timeLoggedInMinutes));
            }

            return Task.CompletedTask;
        }

        public Task<List<ReportDto>> GetAllReportsAsync(int? employeeId, CancellationToken cancellationToken)
        {
            return Task.FromResult(_Reports.Select(pair => new ReportDto
            {
                Id = pair.Key,
                // EmployeeId = pair.Value.Item1,
                // PositionId = pair.Value.Item2, 
                // RoleId = pair.Value.Item3, 
                // TimeLoggedInMinutes = pair.Value.Item4
            }).ToList());
        }
    }
    */

    public class ReportsRepository : IReportsRepository
    {
        private readonly IDatabaseConnectionProvider _databaseConnectionProvider;

        public ReportsRepository(IDatabaseConnectionProvider databaseConnectionProvider)
        {
            _databaseConnectionProvider = databaseConnectionProvider;
        }

        public async Task<long> InsertReport(int projectId, int employeeId, int positionId, int roleId, short timeLoggedInMinutes,
            CancellationToken cancellationToken)
        {
            var dynamicParameters = new DynamicParameters();
            dynamicParameters.Add("fk_projectId", projectId, DbType.Int32, ParameterDirection.Input);
            dynamicParameters.Add("fk_employeeId", employeeId, DbType.Int32, ParameterDirection.Input);
            dynamicParameters.Add("fk_positionId", positionId, DbType.Int32, ParameterDirection.Input);
            dynamicParameters.Add("fk_roleId", roleId, DbType.Int32, ParameterDirection.Input);
            dynamicParameters.Add("timeLoggedMinutes", timeLoggedInMinutes, DbType.Int16, ParameterDirection.Input);

            await using var dbConnection = _databaseConnectionProvider.DbConnection();
            await dbConnection.QueryAsync<long>("[attendance].[REPORT_INSERT]", dynamicParameters, cancellationToken);

            // return dynamicParameters.Get<long>("ReportId");
            return 0;
        }

        public async Task UpdateReport(long reportId, int employeeId, int positionId, int roleId,
            short timeLoggedInMinutes,
            CancellationToken cancellationToken)
        {
            var dynamicParameters = new DynamicParameters();
            dynamicParameters.Add("ProjectId", employeeId, DbType.Int32, ParameterDirection.Input);
            dynamicParameters.Add("ReportId", reportId, DbType.Int32, ParameterDirection.Input);
            dynamicParameters.Add("EmployeeId", employeeId, DbType.Int32, ParameterDirection.Input);
            dynamicParameters.Add("PositionId", positionId, DbType.Int32, ParameterDirection.Input);
            dynamicParameters.Add("RoleId", roleId, DbType.Int32, ParameterDirection.Input);
            dynamicParameters.Add("TimeLoggedInMinutes", timeLoggedInMinutes, DbType.Int16, ParameterDirection.Input);

            await using var dbConnection = _databaseConnectionProvider.DbConnection();
            await dbConnection.QueryAsync<long>("[attendance].[REPORT_UPDATE]", dynamicParameters, cancellationToken);
        }

        public async Task<List<ReportDto>> GetAllReportsAsync(CancellationToken cancellationToken)
        {
            var dynamicParameters = new DynamicParameters();

            await using var dbConnection = _databaseConnectionProvider.DbConnection();
            
            List<ReportDto> reports;
            using (var resultSets = await dbConnection.QueryMultipleAsync(
                       "[attendance].[REPORTS_GET]", dynamicParameters,
                       commandType: CommandType.StoredProcedure))
            {
                reports = resultSets.Read<ReportDto>().ToList();

                if (!reports.Any())
                {
                    return new List<ReportDto>();
                }
            }

            return reports.ToList();
        }

        public async Task<List<ReportDto>> GetReportsByEmployeeIdAsync(int? employeeId, CancellationToken cancellationToken)
        {
            
                var dynamicParameters = new DynamicParameters();

                await using var dbConnection = _databaseConnectionProvider.DbConnection();

                if (employeeId != null)
                {
                    dynamicParameters.Add("EmployeeId", employeeId, DbType.Int32, ParameterDirection.Input);
                }
            
                List<ReportDto> reports;
                using (var resultSets = await dbConnection.QueryMultipleAsync(
                           "[attendance].[REPORTS_GET_BY_EMPLOYEEID]", dynamicParameters,
                           commandType: CommandType.StoredProcedure))
                {
                    reports = resultSets.Read<ReportDto>().ToList();

                    if (!reports.Any())
                    {
                        return new List<ReportDto>();
                    }
                }

                return reports.ToList();
            
        }
    }
}