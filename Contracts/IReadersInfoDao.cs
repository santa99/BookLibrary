using Contracts.Models;

namespace Contracts;

/// <summary>
/// Class <see cref="IReadersInfoDao"/> represents data access object to readers schema.
/// </summary>
public interface IReadersInfoDao
{
    /// <summary>
    /// Gets readers info by the provided <paramref name="readersCardId"/>.
    /// </summary>
    /// <param name="readersCardId">Id of the readers card.</param>
    /// <returns><see cref="ReadersInfo"/> or null if provided card id is not found.</returns>
    ReadersInfo? Read(int readersCardId);
    
    /// <summary>
    /// Provides the readers info list.
    /// </summary>
    /// <returns>List of readers info.</returns>
    List<ReadersInfo> SelectReadersInfos();
}