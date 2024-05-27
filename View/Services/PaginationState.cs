namespace View.Services;

/// <summary>
/// Class <see cref="PaginationState"/> represents and holds all values for the correct pagination.
/// </summary>
public class PaginationState
{
    /// <summary>
    /// Number of element per page.
    /// </summary>
    public int PageSize { get; private set; } = 10;

    /// <summary>
    /// Page number currently set.
    /// </summary>
    public int Page { get; private set; } = 1;

    /// <summary>
    /// Starting element number represents absolute element order to be picked for the current page start.
    /// </summary>
    public int StartWindow { get; private set; }

    /// <summary>
    /// Ending element number represents absolute element order to be picked for the current page ending.
    /// </summary>
    public int EndWindow { get; private set; }

    /// <summary>
    /// Last page available.
    /// </summary>
    public int LastPage => ((int)Math.Ceiling(((decimal)TotalCount / PageSize)));

    /// <summary>
    /// Absolute number of elements available represents all elements possibly displayed across pages.
    /// </summary>
    public int TotalCount { get; set; }

    public PaginationState()
    {
        CalculateWindow(Page, PageSize);
    }

    private void CalculateWindow(int page, int perPage)
    {
        StartWindow = (page - 1) * perPage;
        EndWindow = StartWindow + perPage;
    }

    /// <summary>
    /// Provides a new count of element displayed per page. 
    /// </summary>
    /// <param name="perPage">Number of elements per current page.</param>
    public void SetDisplayCount(int perPage)
    {
        if (perPage < 0)
        {
            return;
        }

        Page = 1;
        PageSize = perPage;
        CalculateWindow(Page, PageSize);
    }

    /// <summary>
    /// Provides a new page to be displayed.
    /// </summary>
    /// <param name="page">Page number.</param>
    public bool SetPage(int page)
    {
        if (page < 1 || page > LastPage)
        {
            return false;
        }

        Page = page;
        CalculateWindow(Page, PageSize);
        return true;
    }
}