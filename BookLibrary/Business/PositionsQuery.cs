using Attendance.DataAccess.Repositories;

namespace Attendance.Business;

public interface IPositionsQuery
{
    public Task<List<(int PositionId, string PositionName)>> GetAllPositions(CancellationToken cancellationToken);
}

public class PositionsQuery : IPositionsQuery
{
    private readonly IReportsRepository _reportsRepository;

    public PositionsQuery(IReportsRepository reportsRepository)
    {
        _reportsRepository = reportsRepository;
    }

    public async Task<List<(int PositionId, string PositionName)>> GetAllPositions(CancellationToken cancellationToken)
    {
        var positions = await _reportsRepository.GetAllPositionsAsync(cancellationToken);

        return positions.Select(dto => (dto.Id, dto.Name)).ToList();
    }
}