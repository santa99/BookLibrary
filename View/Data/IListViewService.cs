namespace View.Data;

/// <summary>
/// Contract defining list view service for displaying list of any items.
/// </summary>
/// <typeparam name="TRecord">Type of record.</typeparam>
public interface IListViewService<out TRecord> : IPaginationService
{
    /// <summary>
    /// Items of the list view service.
    /// </summary>
    public IEnumerable<TRecord> Items { get; }

    /// <summary>
    /// Observer defined to listen to list changed.
    /// </summary>
    public event EventHandler? ListChanged;
    
    /// <summary>
    /// Notifies the change of the list thus <see cref="ListChanged"/> should be fired. 
    /// </summary>
    /// <param name="sender">Event sender.</param>
    public void NotifyListChanged(object? sender);
}