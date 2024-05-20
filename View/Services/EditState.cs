using Contracts.Models;
using Contracts.Models.Responses;
using Microsoft.AspNetCore.Mvc;
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
    }

    public bool IsEntryInEdit(EditableTableEntry editableTableEntry)
    {
        return _editables.Contains(editableTableEntry);
    }

    /// <summary>
    /// Call this method to store previously edited changes.
    /// </summary>
    public async Task SaveChanges()
    {
        var dirty = _editables.Where(entry => entry.IsDirty).ToList();
        if (!dirty.Any())
        {
            IsEditMode = false;
            return;
        }

        var completed = new List<EditableTableEntry>();

        foreach (var editableTableEntry in dirty)
        {
            if (await Save(editableTableEntry.Book, editableTableEntry.Save, editableTableEntry.DisplayError))
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

        OnSaveCompleted?.Invoke(this, true);
    }

    /// <summary>
    /// Call this method to restore previous state before editing.
    /// </summary>
    public async Task DiscardChanges()
    {
        var dirty = _editables.Where(entry => entry.IsDirty).ToList();
        if (!dirty.Any())
        {
            IsEditMode = false;
            return;
        }

        var completed = new List<EditableTableEntry>();

        foreach (var editableTableEntry in dirty)
        {
            if (await Discard(editableTableEntry.Book, editableTableEntry.Save, editableTableEntry.DisplayError))
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

        OnDiscardCompleted?.Invoke(this, true);
    }

    /// <summary>
    /// Remove all selected entries.
    /// </summary>
    public async Task RemoveSelected()
    {
        var check = _editables.Where(entry => entry.Checked).ToList();
        if (!check.Any())
        {
            IsEditMode = false;
            return;
        }

        var completed = new List<EditableTableEntry>();

        foreach (var editableTableEntry in check)
        {
            if (await Remove(editableTableEntry.Book, editableTableEntry.Remove, editableTableEntry.DisplayError))
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

        OnRemovalCompleted?.Invoke(this, true);
    }

    private async Task<bool> Save(BookModel book, Action<BookModel> onSuccess, Action<ErrorCodeModel> onFailure)
    {
        var updatedBook = await BooksService.UpdateBook(book);

        switch (updatedBook)
        {
            case OkObjectResult { Value: BookModel bookModel }:
                onSuccess.Invoke(bookModel);
                return true;
            case BadRequestObjectResult { Value: ErrorCodeModel errorCodeModel }:
                onFailure.Invoke(errorCodeModel);
                return false;
        }

        return false;
    }

    private async Task<bool> Discard(BookModel book, Action<BookModel> onSuccess, Action<ErrorCodeModel> onFailure)
    {
        var revertedBook = await BooksService.GetBook(book.Id);
        if (revertedBook == null)
        {
            onFailure.Invoke(new ErrorCodeModel(-1, "Define in GetBook.", ""));
            return false;
        }

        onSuccess.Invoke(revertedBook);
        return true;
    }

    private async Task<bool> Remove(BookModel book, Action onSuccess, Action<ErrorCodeModel> onFailure)
    {
        var removeBook = await BooksService.RemoveBook(book);
        if (!removeBook)
        {
            onFailure.Invoke(new ErrorCodeModel(-1, "Define in RemoveBook.", ""));
            return false;
        }

        onSuccess.Invoke();
        return true;
    }
}