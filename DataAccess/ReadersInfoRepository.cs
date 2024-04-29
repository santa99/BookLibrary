using Contracts;

namespace DataAccess;

/// <summary>
/// Readers info repository.
/// </summary>
public class ReadersInfoRepository : IReadersInfoRepository
{
    private readonly IReadersInfoDao _readersInfoDao;

    public ReadersInfoRepository(IReadersInfoDao readersInfoDao)
    {
        _readersInfoDao = readersInfoDao;
    }

    public Task<ReadersInfo?> GetReadersInfo(int readersCardId, CancellationToken cancellationToken)
    {
        return Task.FromResult(_readersInfoDao.Read(readersCardId));
    }

    public Task<List<ReadersInfo>> ListReadersInfo(CancellationToken cancellationToken)
    {
        return Task.FromResult(_readersInfoDao.getReadersInfos());
    }
}