using Contracts;

namespace DataAccess;

public class ReadersInfoRepository : IReadersInfoRepository
{
    private readonly XmlDatabase _xmlDatabase;

    public ReadersInfoRepository(XmlDatabase xmlDatabase)
    {
        _xmlDatabase = xmlDatabase;
    }

    public ReaderInfo? GetReadersInfo(int readersCardId)
    {
        return _xmlDatabase.GetReadersInfo(readersCardId);
    }
}