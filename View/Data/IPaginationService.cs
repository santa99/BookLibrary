using View.Services;

namespace View.Data;

/// <summary>
/// Pagination service provides basic set of operations and access to the pagination state.
/// </summary>
public interface IPaginationService
{
    /// <summary>
    /// Event handler fired on page changed right after new items have been loaded.
    /// </summary>
    public event EventHandler<int>? PageChanged;

    /// <summary>
    /// Pagination state provides state data of the pagination.
    /// </summary>
    public PaginationState PaginationState { get; }

    /// <summary>
    /// Call this method to request for a new page or
    /// due to change in number of displayed items or any other reason for that needs
    /// to handle pagination.
    /// </summary>
    /// <param name="sender">Sender.</param>
    /// <param name="page">Page number to display.</param>
    public void NotifyPageChanged(object? sender, int page);
}