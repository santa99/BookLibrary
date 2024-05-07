using Microsoft.AspNetCore.Components;

namespace View.Services;

public class PaginationState
{
    /// <summary>
    /// 
    /// </summary>
    public int Start { get; set; } = 0;

    /// <summary>
    /// 
    /// </summary>
    public int Count { get; set; } = 2;

    /// <summary>
    /// Set a new state of the pagination.
    /// </summary>
    /// <param name="val"></param>
    public async Task SetPagination(ChangeEventArgs  val)
    {
        if (int.TryParse((string?)val.Value, out var newCount))
        {
            Count = newCount;
            await PageChanged.InvokeAsync();
        }
    }

    [Parameter]
    public EventCallback PageChanged { get; set; } = new EventCallback();
}