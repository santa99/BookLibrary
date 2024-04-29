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

    public ReadersInfo? GetReadersInfo(int readersCardId)
    {
        return _readersInfoDao.Read(readersCardId);
    }
}