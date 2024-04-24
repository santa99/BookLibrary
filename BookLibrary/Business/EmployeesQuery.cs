using Attendance.DataAccess.Repositories;

namespace Attendance.Business;

public interface IEmployeesQuery
{
    public Task<List<(int EmployeeId, string Employee)>> GetAllEmployees(CancellationToken cancellationToken);
}

public class EmployeesQuery : IEmployeesQuery
{
    private readonly IReportsRepository _reportsRepository;

    public EmployeesQuery(IReportsRepository reportsRepository)
    {
        _reportsRepository = reportsRepository;
    }

    public async Task<List<(int EmployeeId, string Employee)>> GetAllEmployees(CancellationToken cancellationToken)
    {
        var employees = await _reportsRepository.GetAllEmployeesAsync(cancellationToken);

        return employees.Select(dto => (dto.Id, dto.Name)).ToList();
    }
}