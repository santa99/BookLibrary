namespace Contracts;

/// <summary>
/// Contract so that the API can access data from readers info repository.
/// </summary>
public interface IReadersInfoRepository
{
    /// <summary>
    /// Retrieves readers info based on the provided <paramref name="readersCardId"/>.
    /// </summary>
    /// <param name="readersCardId">Unique readers card id.</param>
    /// <returns><see cref="ReadersInfo"/> or null if provided id hasn't been found.</returns>
    ReadersInfo? GetReadersInfo(int readersCardId);
}