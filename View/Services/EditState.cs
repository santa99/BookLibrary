﻿using Microsoft.AspNetCore.Components;
using View.Shared;

namespace View.Services;

public class EditState
{
    public BooksService BooksService { get; set; }
    
    /// <summary>
    /// Checks whether in edit mode.
    /// </summary>
    public bool IsEditMode { get; private set; }
    
    /// <summary>
    /// Checks whether any of the displayed item has been previously edited.
    /// </summary>
    public bool IsDirty { get; set; }

    public async Task EnterEditMode()
    {
        IsEditMode = true;
    }

    public async Task ExitEditMode()
    {
        IsEditMode = false;
    }

    private readonly List<EditableTableEntry> _editables = new();

    public void StartEditEntry(EditableTableEntry editableTableEntry)
    {
        if (IsMeEdit(editableTableEntry))
        {
            return;
        }
        
        _editables.Add(editableTableEntry);

        IsDirty = true;
    }

    public bool IsMeEdit(EditableTableEntry editableTableEntry)
    {
        return _editables.Contains(editableTableEntry);
    }

    public async Task SaveChanges()
    {
        var copy = new List<EditableTableEntry>(_editables);
        foreach (var editableTableEntry in copy)
        {
            await editableTableEntry.SaveChanges(BooksService);
        }

        _editables.RemoveAll(entry => copy.Contains(entry));
    }

    public async Task DiscardChanges()
    {
        var copy = new List<EditableTableEntry>(_editables);
        foreach (var editableTableEntry in copy)
        {
            await editableTableEntry.DiscardChanges();
        }
        
        _editables.RemoveAll(entry => copy.Contains(entry));
    }
}