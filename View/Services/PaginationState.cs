namespace View.Services;

public class PaginationState
{
    /// <summary>
    /// Number of element per page.
    /// </summary>
    public int Count { get; private set; } = 1;

    /// <summary>
    /// Page number.
    /// </summary>
    public int Page { get; private set; } = 1;

    /// <summary>
    /// Starting element number.
    /// </summary>
    public int StartWindow { get; private set; }

    /// <summary>
    /// Ending element number.
    /// </summary>
    public int EndWindow { get; private set; }
    
    /// <summary>
    /// Actual returned count per page.
    /// </summary>
    public int Available
    {
        set
        {
            IsPrevAvailable = value >= 0;
            IsNextAvailable = value >= Count;
        }
    }

    /// <summary>
    /// Call this method to test whether next page is available.
    /// </summary>
    public bool IsNextAvailable { get; private set; }
    
    /// <summary>
    /// Call this method to test whether prev page is available.
    /// </summary>
    public bool IsPrevAvailable { get; private set; }

    public PaginationState()
    {
        CalculateWindow(Page, Count);
    }

    private void CalculateWindow(int page, int perPage)
    {
        StartWindow = (page - 1) * perPage;
        EndWindow = StartWindow + perPage;
    }

    public void SetDisplayCount(int perPage)
    {
        if (perPage < 0)
        {
            return;
        }

        Count = perPage;
        CalculateWindow(Page, Count);
    }

    public void SetPage(int page)
    {
        if (page < 1)
        {
            return;
        }

        Page = page;
        CalculateWindow(Page, Count);
    }
}