using Contracts;

namespace DataAccess;

/// <summary>
/// Dummy implementation of the <see cref="IReadersInfoDao"/>.
/// </summary>
public class ReadersInfoDaoImpl : IReadersInfoDao
{
    private readonly Dictionary<int, Tuple<string, string>> _readersInfo = new()
    {
        { 1, new Tuple<string, string>("Marek", "Blanar") },
        { 2, new Tuple<string, string>("Ivan", "Maly") },
        { 3, new Tuple<string, string>("Tibor", "Vesely") }
    };

    public ReaderInfo? Read(int readersCardId)
    {
        if (!_readersInfo.TryGetValue(readersCardId, out var reader))
        {
            return null;
        }

        return new ReaderInfo
        {
            ReaderCardId = readersCardId,
            Name = reader.Item1,
            LastName = reader.Item2
        };
    }
}