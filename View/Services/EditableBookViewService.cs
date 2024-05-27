using Contracts.Models;
using Contracts.Models.Responses;
using Microsoft.AspNetCore.Mvc;
using View.Data;

namespace View.Services;

public class EditableBookViewService : BookViewService, IEditableListViewService<BookModel>
{
    public event EventHandler<BookModel>? ItemAdded;
    public event EventHandler<BookModel>? ItemChanged;
    public event EventHandler<Tuple<BookModel, bool>>? ItemSelected;
    public event EventHandler<bool>? EditModeEntered;
    public event EventHandler<(BookModel, ErrorCodeModel)>? OnItemError;
    public event EventHandler<bool>? EditFinished;
    public EditableState EditableState { get; } = new();
    public IEnumerable<BookModel> EditableItems => _editableItems;

    private List<BookModel> _editableItems = new();

    /// <inheritdoc />
    public EditableBookViewService(BooksService booksService) : base(booksService)
    {
        ListChanged += (_, _) => { _editableItems = new List<BookModel>(Items); };
    }

    public void NotifyItemChanged(object? sender, BookModel item)
    {
        if (EditableState.EditList.Contains(item))
        {
            return;
        }

        EditableState.EditList.Add(item);
        // ItemChanged?.Invoke(sender, item);
        // NotifyPageChanged(this, PaginationState.Page);
    }

    public void NotifyItemSelected(object? sender, BookModel item, bool selected)
    {
        if (selected)
        {
            EditableState.Select(item);
        }
        else
        {
            EditableState.Unselect(item);
        }

        ItemSelected?.Invoke(this, new Tuple<BookModel, bool>(item1: item, item2: selected));
    }

    public void NotifyEditModeEntered(object? sender, bool selected)
    {
        if (EditableState.IsEditMode == selected)
        {
            return;
        }
        
        if (!EditableState.IsEditMode && selected)
        {
            EditableState.EditList.Clear();
            EditableState.SelectionList.Clear();
        }

        EditableState.IsEditMode = selected;

        EditModeEntered?.Invoke(this, EditableState.IsEditMode);
    }

    public async Task RemoveSelected()
    {
        var toProcess = new List<BookModel>(EditableState.SelectionList);
        var completed = new List<BookModel>();
        
        foreach (var model in toProcess)
        {
            if (await Remove(model, null, codeModel => OnItemFail(model, codeModel)))
            {
                completed.Add(model);
            }
        }

        EditableState.SelectionList.RemoveAll(entry => toProcess.Contains(entry));

        if (completed.Count != toProcess.Count)
        {
            EditFinished?.Invoke(this, false);
            return;
        }

        EditFinished?.Invoke(this, true);
        NotifyPageChanged(this, PaginationState.Page);
    }

    public async Task SaveAllChanges()
    {
        var toProcess = new List<BookModel>(EditableState.EditList.ToList());
        if (!toProcess.Any())
        {
            return;
        }

        var completed = new List<BookModel>();

        foreach (var bookModel in toProcess)
        {
            if (await Save(bookModel, SaveCompleted, codeModel => OnItemFail(bookModel, codeModel)))
            {
                completed.Add(bookModel);
            }
        }

        EditableState.EditList.RemoveAll(entry => completed.Contains(entry));

        if (completed.Count != toProcess.Count)
        {
            EditFinished?.Invoke(this, false);
            return;
        }

        EditFinished?.Invoke(this, true);
        NotifyPageChanged(this, PaginationState.Page);
    }

    public async Task RestoreChanges()
    {
        var toProcess = new List<BookModel>(EditableState.EditList.ToList());
        if (!toProcess.Any())
        {
            return;
        }

        var completed = new List<BookModel>();

        foreach (var bookModel in toProcess)
        {
            if (await Restore(bookModel, SaveCompleted, codeModel => OnItemFail(bookModel, codeModel)))
            {
                completed.Add(bookModel);
            }
        }

        EditableState.EditList.RemoveAll(entry => completed.Contains(entry));

        NotifyPageChanged(this, PaginationState.Page);
    }

    private void SaveCompleted(BookModel bookModel)
    {
        ItemChanged?.Invoke(this, bookModel);
    }

    public void AddItem(BookModel item)
    {
        if (EditableState.EditList.Contains(item))
        {
            return;
        }

        EditableState.EditList.Add(item);
        _editableItems.Insert(Math.Min(PaginationState.EndWindow - 1, _editableItems.Count), item);
        ItemAdded?.Invoke(this, item);
    }

    public async Task RemoveItem(BookModel item)
    {
        if (await Remove(item, () => { }, code => OnItemFail(item, code)))
        {
            EditableState.SelectionList.Remove(item);
        }
    }
    
    public async Task Discard(BookModel item)
    {
        if (await Restore(item, SaveCompleted, code => OnItemFail(item, code)))
        {
            EditableState.EditList.Remove(item);
        }
    }

    public async Task SaveItem(BookModel item)
    {
        if (await Save(item, SaveCompleted, codeModel => OnItemFail(item, codeModel)))
        {
            EditableState.EditList.Remove(item);
        }
    }

    private void OnItemFail(BookModel bookModel, ErrorCodeModel errorCodeModel)
    {
        OnItemError?.Invoke(this, new ValueTuple<BookModel, ErrorCodeModel>(bookModel, errorCodeModel));
    }

    private async Task<bool> Save(BookModel book, Action<BookModel>? onSuccess, Action<ErrorCodeModel>? onFailure)
    {
        var updatedBook = await BooksService.UpdateBook(book);
        if (updatedBook == null)
        {
            onFailure?.Invoke(new ErrorCodeModel(-1, "Save failed.", "Internal client error."));
            return false;
        }

        switch (updatedBook)
        {
            case OkObjectResult { Value: BookModel bookModel }:
                onSuccess?.Invoke(bookModel);
                return true;
            case BadRequestObjectResult { Value: ErrorCodeModel errorCodeModel }:
                onFailure?.Invoke(errorCodeModel);
                return false;
        }

        return false;
    }

    private async Task<bool> Remove(BookModel book, Action? onSuccess, Action<ErrorCodeModel>? onFailure)
    {
        var removeBook = await BooksService.RemoveBook(book);
        if (!removeBook)
        {
            onFailure?.Invoke(new ErrorCodeModel(-1, "Define in RemoveBook.", "Internal client error."));
            return false;
        }

        onSuccess?.Invoke();
        return true;
    }

    private async Task<bool> Restore(BookModel book, Action<BookModel>? onSuccess, Action<ErrorCodeModel>? onFailure)
    {
        var actionResult = await BooksService.GetBook(book.Id);
        if (actionResult == null)
        {
            onFailure?.Invoke(new ErrorCodeModel(-1, "Restore failed.", "Internal client error."));
            return false;
        }

        switch (actionResult)
        {
            case OkObjectResult { Value: BookModel bookModel }:
                onSuccess?.Invoke(bookModel);
                return true;
            case BadRequestObjectResult { Value: ErrorCodeModel errorCodeModel }:
                onFailure?.Invoke(errorCodeModel);
                return false;
        }

        return false;
    }
}