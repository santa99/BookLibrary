using Attendance.DataAccess.Dtos;
using Attendance.DataAccess.Repositories;

namespace Attendance.Business;

public interface IGetReportsQuery
{
    public Task<List<ReportDto>> GetReportsAsync(CancellationToken cancellationToken);
    public Task<List<(string ProjectName, int Prize, int TotalTime)>> GetReportsByProjectAsync(int projectId, DateTimeOffset from, DateTimeOffset to, CancellationToken cancellationToken);
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
        var allReportsAsync = await _reportsRepository.GetAllReportsAsync(cancellationToken);

        return allReportsAsync;
    }

    public async Task<List<(string ProjectName, int Prize, int TotalTime)>> GetReportsByProjectAsync(int projectId, DateTimeOffset from, DateTimeOffset to,
        CancellationToken cancellationToken)
    {
        var allReportsAsync = await _reportsRepository.GetAllReportsAsync(cancellationToken);

        var reportDtos = allReportsAsync
            .Where(dto => dto.WhenItHappend <= to && dto.WhenItHappend >= from)
            .GroupBy(dto => dto.Project)
            .Select(g => new
            {
                ProjectName = g.Key,
                Prize = g.Sum(x => x.WorkedAsCost * x.HowLong),
                TotalTime = g.Sum(s => s.HowLong),
            }).ToList();

        var valueTuples = reportDtos.Select(arg => (arg.ProjectName, Prize: arg.Prize, TotalTime: arg.TotalTime)).ToList();
        return valueTuples;
    }
}