using Attendance.DataAccess.Repositories;

namespace Attendance.Business;

public interface IProjectsQuery
{
    public Task<List<(int ProjectId, string Project)>> GetAllEmployees(CancellationToken cancellationToken);
}

public class ProjectsQuery : IProjectsQuery
{
    private readonly IReportsRepository _reportsRepository;

    public ProjectsQuery(IReportsRepository reportsRepository)
    {
        _reportsRepository = reportsRepository;
    }

    public async Task<List<(int ProjectId, string Project)>> GetAllEmployees(CancellationToken cancellationToken)
    {
        var projects = await _reportsRepository.GetAllProjectsAsync(cancellationToken);

        return projects.Select(dto => (dto.Id, dto.Name)).ToList();
    }
}