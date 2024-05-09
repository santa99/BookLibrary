namespace View.Services;

public class PaginationState
{
    /// <summary>
    /// Start element.
    /// </summary>
    public int Start => StartWindow;

    /// <summary>
    /// Element count to read.
    /// </summary>
    public int Count { get; private set; } = 2;

    public int Page { get; private set; } = 1;

    public int StartWindow { get; private set; }

    public int EndWindow { get; private set; }

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