namespace Contracts;

public interface IReadersInfoRepository
{
    ReaderInfo? GetReadersInfo(int readersCardId);
}