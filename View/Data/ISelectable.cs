namespace View.Data;

public interface ISelectable<TRecord> 
{
    /// <summary>
    /// Event handler fired on item selected.
    /// </summary>
    public event EventHandler<TRecord>? ItemSelected;
    
    /// <summary>
    /// Call this method to notify selection of specific item.
    /// </summary>
    /// <param name="sender">Sender.</param>
    /// <param name="id">Item that has been modified.</param>
    public Task NotifyItemSelected(object? sender, TRecord id);
}