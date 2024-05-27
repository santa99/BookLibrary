using Contracts.Models.Responses;
using View.Services;

namespace View.Data;

public interface IEditableListViewService<TRecord> : IListViewService<TRecord>
{
    /// <summary>
    /// Editable items enumeration.
    /// </summary>
    public IEnumerable<TRecord> EditableItems { get; }
    
    /// <summary>
    /// Event handler fired on new item added.
    /// </summary>
    public event EventHandler<TRecord>? ItemAdded;
    
    /// <summary>
    /// Add new item into the list.
    /// </summary>
    /// <param name="item">Item to be added.</param>
    void AddItem(TRecord item);
    
    /// <summary>
    /// Edit finished event handler.
    /// </summary>
    public event EventHandler<bool> EditFinished; 
    
    /// <summary>
    /// Event handler fired on item change.
    /// </summary>
    public event EventHandler<TRecord>? ItemChanged;
    
    /// <summary>
    /// Call this method to notify change of specific item.
    /// </summary>
    /// <param name="sender">Sender.</param>
    /// <param name="item">Item that has been modified.</param>
    public void NotifyItemChanged(object? sender, TRecord item);

    /// <summary>
    /// Event handler fired on item selection.
    /// </summary>
    public event EventHandler<Tuple<TRecord, bool>>? ItemSelected;
    
    /// <summary>
    /// Call this method to notify editable list that item has been selected.
    /// </summary>
    /// <param name="sender">Sender.</param>
    /// <param name="item">Item to be selected.</param>
    /// <param name="selected">New state of selection.</param>
    public void NotifyItemSelected(object? sender, TRecord item, bool selected);
    
    /// <summary>
    /// Editable state of the list.
    /// </summary>
    public EditableState EditableState { get; }

    /// <summary>
    /// Edit mode entered.
    /// </summary>
    public event EventHandler<bool> EditModeEntered;
    
    public void NotifyEditModeEntered(object? sender, bool selected);

    public event EventHandler<(TRecord, ErrorCodeModel)> OnItemError;

    /// <summary>
    /// Remove provided item from the list.
    /// </summary>
    /// <param name="item">Item to be removed.</param>
    Task RemoveItem(TRecord item);

    /// <summary>
    /// Discard changes over the provided item.
    /// </summary>
    /// <param name="item">Item to be discarded.</param>
    Task Discard(TRecord item);

    /// <summary>
    /// Save changes over provided item.
    /// </summary>
    /// <param name="item">Item to be saved.</param>
    Task SaveItem(TRecord item);

    /// <summary>
    /// Removes all items that have been selected.
    /// </summary>
    /// <returns></returns>
    public Task RemoveSelected();
    
    /// <summary>
    /// Save all items that have been changed.
    /// </summary>
    public Task SaveAllChanges();
    
    /// <summary>
    /// Restore all items of that have been changed.
    /// </summary>
    public Task RestoreChanges();
}