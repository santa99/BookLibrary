using View.Shared;

namespace View.Services;

/// <summary>
/// Class <see cref="EditState"/> serves as a state for editable entries. It can update or discard book
/// properties that have been previously changed.
/// </summary>
public class EditState
{
    private readonly List<EditableTableEntry> _editables = new();
    
    /// <summary>
    /// Book service for basic crud operations with books.
    /// </summary>
    public BooksService BooksService { get; set; }
    
    /// <summary>
    /// Checks whether in edit mode.
    /// </summary>
    public bool IsEditMode { get; private set; }
    
    /// <summary>
    /// Checks whether any of the displayed item has been previously changed.
    /// </summary>
    public bool IsDirty { get; private set; }

    /// <summary>
    /// Checks whether any of the displayed items has been checked.
    /// </summary>
    public bool IsAnySelected => _editables.Any(entry => entry.Checked);

    /// <summary>
    /// Callback when the save of the edit ended with success or fail.
    /// </summary>
    public EventHandler<bool>? OnSaveCompleted { get; set; }
    
    /// <summary>
    /// Callback fired when the discard ended with success or fail.
    /// </summary>
    public EventHandler<bool>? OnDiscardCompleted { get; set; }
    
    /// <summary>
    /// Callback fired when checked items have been removed.
    /// </summary>
    public EventHandler<bool>? OnRemovalCompleted { get; set; }

    /// <summary>
    /// Call this method when edit mode entered.
    /// </summary>
    public async Task EnterEditMode()
    {
        IsEditMode = true;
    }

    public void StartEditEntry(EditableTableEntry editableTableEntry)
    {
        if (IsEntryInEdit(editableTableEntry))
        {
            return;
        }
        
        _editables.Add(editableTableEntry);

        IsDirty = true;
    }

    public bool IsEntryInEdit(EditableTableEntry editableTableEntry)
    {
        return _editables.Contains(editableTableEntry);
    }

    public async Task SaveChanges()
    {
        var dirty = new List<EditableTableEntry>(_editables.Where(entry => entry.IsDirty));
        var completed = new List<EditableTableEntry>();
        
        foreach (var editableTableEntry in dirty)
        {
            if (await editableTableEntry.SaveChanges(BooksService))
            {
                completed.Add(editableTableEntry);
            }
        }

        _editables.RemoveAll(entry => completed.Contains(entry));
        
        if (dirty.Count != completed.Count)
        {
            OnSaveCompleted?.Invoke(this, false);
            return;
        }
        
        IsEditMode = false;
        IsDirty = false;
        
        OnSaveCompleted?.Invoke(this,true);
    }

    public async Task DiscardChanges()
    {
        if (!IsDirty)
        {
            IsEditMode = false;
            return;
        }
        
        var dirty = new List<EditableTableEntry>(_editables.Where(entry => entry.IsDirty));
        var completed = new List<EditableTableEntry>();
        
        foreach (var editableTableEntry in dirty)
        {
            if (await editableTableEntry.DiscardChanges(BooksService))
            {
                completed.Add(editableTableEntry);
            }
        }

        _editables.RemoveAll(entry => completed.Contains(entry));
        
        if (dirty.Count != completed.Count)
        {
            OnDiscardCompleted?.Invoke(this, false);
            return;
        }
        
        IsEditMode = false;
        IsDirty = false;
        
        OnDiscardCompleted?.Invoke(this, true);
    }

    public async Task RemoveSelected()
    {
        var check = new List<EditableTableEntry>(_editables.Where(entry => entry.Checked));
        var completed = new List<EditableTableEntry>();
        
        foreach (var editableTableEntry in check)
        {
            if (await editableTableEntry.Remove(BooksService))
            {
                completed.Add(editableTableEntry);
            }
        }

        _editables.RemoveAll(entry => check.Contains(entry));
        
        if (check.Count != completed.Count)
        {
            OnRemovalCompleted?.Invoke(this, false);
            return;
        }
        
        IsEditMode = false;
        IsDirty = false;
        
        OnRemovalCompleted?.Invoke(this, true);
    }
}