using Attendance.DataAccess.Dtos;
using Attendance.DataAccess.Repositories;

namespace Attendance.Business;

public interface IGetReportsQuery
{
    public Task<List<ReportDto>> GetReportsAsync(CancellationToken cancellationToken);

    public Task<List<(string ProjectName, decimal TotalPrize, double TotalTime)>> GetReportsByProjectAsync(
        int? projectId,
        DateTimeOffset? from, DateTimeOffset? to, CancellationToken cancellationToken);
}

public class GetReportsQuery : IGetReportsQuery
{
    private readonly IReportsRepository _reportsRepository;

    public GetReportsQuery(IReportsRepository reportsRepository)
    {
        _reportsRepository = reportsRepository;
    }


    public async Task<List<ReportDto>> GetReportsAsync(CancellationToken cancellationToken)
    {
        return await _reportsRepository.GetAllReportsAsync(cancellationToken);;
    }

    public async Task<List<(string ProjectName, decimal TotalPrize, double TotalTime)>> GetReportsByProjectAsync(
        int? projectId,
        DateTimeOffset? from, DateTimeOffset? to,
        CancellationToken cancellationToken)
    {
        var allReportsAsync = await _reportsRepository.GetAllReportsAsync(cancellationToken);

        // Project fitment
        if (projectId != null && projectId > 0)
        {
            allReportsAsync = allReportsAsync
                .Where(dto => dto.ProjectId == projectId)
                .ToList();
        }
        
        // Date fitment
        var reportDtos = allReportsAsync
            .Where(dto => dto.LogDate <= to && dto.LogDate >= from)
            .GroupBy(dto => dto.Project)
            .Select(g => new
            {
                ProjectName = g.Key,
                TotalPrize = g.Sum(x => x.CostPerHour * (x.LogTimeMinutes / 60.0M)),
                TotalTime = g.Sum(s => s.LogTimeMinutes / 60.0),
            }).ToList();

        var valueTuples = reportDtos
            .Select(arg => (arg.ProjectName, Prize: arg.TotalPrize, TotalTime: arg.TotalTime))
            .ToList();
        return valueTuples;
    }
}