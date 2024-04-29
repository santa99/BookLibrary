using Contracts.Models;

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
    /// <param name="cancellationToken"><see cref="CancellationToken"/></param>
    /// <returns><see cref="ReadersInfo"/> or null if provided id hasn't been found.</returns>
    Task<ReadersInfo?> GetReadersInfo(int readersCardId, CancellationToken cancellationToken);

    /// <summary>
    /// Selects all registered readers.
    /// </summary>
    /// <param name="cancellationToken"><see cref="CancellationToken"/></param>
    /// <returns><see cref="ReadersInfo"/> list</returns>
    public Task<List<ReadersInfo>> ListReadersInfo(CancellationToken cancellationToken);
}