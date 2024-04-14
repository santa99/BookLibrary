using Attendance.DataAccess.Repositories;

namespace Attendance.Business;

public interface IInsertReportCommand
{
    public Task SetReportAsync(int projectId, int employeeId, int positionId, int timeLoggedInMinutes,
        CancellationToken cancellationToken);
}

public class InsertReportCommand : IInsertReportCommand
{
    private readonly IReportsRepository _reportsRepository;

    public InsertReportCommand(IReportsRepository reportsRepository)
    {
        _reportsRepository = reportsRepository;
    }

    public async Task SetReportAsync(int projectId, int employeeId, int positionId,
        int timeLoggedInMinutes, CancellationToken cancellationToken)
    {
        var currentDate = DateTime.Now;
        var allReportsAsync = await _reportsRepository.GetAllReportsAsync(cancellationToken);

        var enumerable = allReportsAsync
            .Where(dto => dto.LogDate.Day <= currentDate.Day)
            .GroupBy(dto => dto.Employee)
            .Select(
                g => new
                {
                    Value = g.Sum(s => s.LogTimeMinutes),
                }).FirstOrDefault();

        if (enumerable?.Value > 480)
        {
            throw new InvalidOperationException(
                $"You have reached level. User have logged {(enumerable.Value / 60.0)}h");
        }

        await _reportsRepository.InsertReport(projectId, employeeId, positionId, timeLoggedInMinutes,
            cancellationToken);
    }
}