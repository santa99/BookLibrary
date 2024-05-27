namespace View.Data;

/// <summary>
/// Represent single query for retrieving items total count and whether the action succeded.
/// </summary>
/// <param name="Items">Items retrieved.</param>
/// <param name="TotalCount">Total count of items may differ from items retrieved.</param>
/// <param name="Successful">True on success.</param>
/// <param name="Message">Optional message</param>
/// <typeparam name="TRecord">Type of record in retrieved list.</typeparam>
public record ItemsQueryResult<TRecord>(
    IEnumerable<TRecord> Items,
    int TotalCount,
    bool Successful,
    string? Message = null) where TRecord : class;