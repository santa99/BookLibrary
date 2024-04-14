using System.Data;
using Attendance.DataAccess.Dtos;
using Attendance.DataAccess.Utilities;
using Dapper;

namespace Attendance.DataAccess.Repositories
{
    public interface IReportsRepository
    {
        Task<long> InsertReport(
            int projectId, int employeeId, int positionId, int timeLoggedInMinutes,
            CancellationToken cancellationToken);

        Task UpdateReport(long reportId, int projectId, int employeeId, int positionId,
            short timeLoggedInMinutes,
            CancellationToken cancellationToken);

        Task<List<ReportDto>> GetAllReportsAsync(CancellationToken cancellationToken);

        Task<List<EmployeeDto>> GetAllEmployeesAsync(CancellationToken cancellationToken);

        Task<List<ProjectDto>> GetAllProjectsAsync(CancellationToken cancellationToken);

        Task<List<PositionDto>> GetAllPositionsAsync(CancellationToken cancellationToken);
    }

    public class ReportsRepository : IReportsRepository
    {
        private readonly IDatabaseConnectionProvider _databaseConnectionProvider;

        public ReportsRepository(IDatabaseConnectionProvider databaseConnectionProvider)
        {
            _databaseConnectionProvider = databaseConnectionProvider;
        }

        public async Task<long> InsertReport(int projectId, int employeeId, int positionId, int timeLoggedInMinutes,
            CancellationToken cancellationToken)
        {
            var dynamicParameters = new DynamicParameters();
            dynamicParameters.Add("ProjectId", projectId, DbType.Int32, ParameterDirection.Input);
            dynamicParameters.Add("EmployeeId", employeeId, DbType.Int32, ParameterDirection.Input);
            dynamicParameters.Add("PositionId", employeeId, DbType.Int32, ParameterDirection.Input);
            dynamicParameters.Add("TimeLoggedMinutes", timeLoggedInMinutes, DbType.Int32, ParameterDirection.Input);

            await using var dbConnection = _databaseConnectionProvider.DbConnection();
            await dbConnection.QueryAsync<long>("[attendance].[REPORT_INSERT]", dynamicParameters, cancellationToken);

            // return dynamicParameters.Get<long>("ReportId");
            return 0;
        }

        public async Task UpdateReport(long reportId, int projectId, int employeeId, int positionId,
            short timeLoggedInMinutes,
            CancellationToken cancellationToken)
        {
            var dynamicParameters = new DynamicParameters();
            dynamicParameters.Add("ProjectId", employeeId, DbType.Int32, ParameterDirection.Input);
            dynamicParameters.Add("ReportId", reportId, DbType.Int32, ParameterDirection.Input);
            dynamicParameters.Add("EmployeeId", employeeId, DbType.Int32, ParameterDirection.Input);
            dynamicParameters.Add("TimeLoggedInMinutes", timeLoggedInMinutes, DbType.Int16, ParameterDirection.Input);

            await using var dbConnection = _databaseConnectionProvider.DbConnection();
            await dbConnection.QueryAsync<long>("[attendance].[REPORT_UPDATE]", dynamicParameters, cancellationToken);
        }
        
        public async Task<List<EmployeeDto>> GetAllEmployeesAsync(CancellationToken cancellationToken)
        {
            var dynamicParameters = new DynamicParameters();

            await using var dbConnection = _databaseConnectionProvider.DbConnection();

            List<EmployeeDto> reports;
            using (var resultSets = await dbConnection.QueryMultipleAsync(
                       "[attendance].[EMPLOYEES_GET]", dynamicParameters,
                       commandType: CommandType.StoredProcedure))
            {
                reports = resultSets.Read<EmployeeDto>().ToList();

                if (!reports.Any())
                {
                    return new List<EmployeeDto>();
                }
            }

            return reports.ToList();
        }
        
        public async Task<List<ProjectDto>> GetAllProjectsAsync(CancellationToken cancellationToken)
        {
            var dynamicParameters = new DynamicParameters();

            await using var dbConnection = _databaseConnectionProvider.DbConnection();

            List<ProjectDto> reports;
            using (var resultSets = await dbConnection.QueryMultipleAsync(
                       "[attendance].[PROJECTS_GET]", dynamicParameters,
                       commandType: CommandType.StoredProcedure))
            {
                reports = resultSets.Read<ProjectDto>().ToList();

                if (!reports.Any())
                {
                    return new List<ProjectDto>();
                }
            }

            return reports.ToList();
        }

        public async Task<List<PositionDto>> GetAllPositionsAsync(CancellationToken cancellationToken)
        {
            var dynamicParameters = new DynamicParameters();

            await using var dbConnection = _databaseConnectionProvider.DbConnection();

            List<PositionDto> reports;
            using (var resultSets = await dbConnection.QueryMultipleAsync(
                       "[attendance].[POSITIONS_GET]", dynamicParameters,
                       commandType: CommandType.StoredProcedure))
            {
                reports = resultSets.Read<PositionDto>().ToList();

                if (!reports.Any())
                {
                    return new List<PositionDto>();
                }
            }

            return reports.ToList();
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
    }
}