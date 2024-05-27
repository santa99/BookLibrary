using Contracts.Models;

namespace View.Services;

/// <summary>
/// Class <see cref="EditableState"/> hold important information related to edited list.
/// </summary>
public class EditableState
{
    /// <summary>
    /// Checks whether in edit mode.
    /// </summary>
    internal bool IsEditMode { get; set; }

    /// <summary>
    /// Is item in edit mode.
    /// </summary>
    /// <param name="bookModel"></param>
    /// <returns></returns>
    public bool IsItemInEdit(BookModel bookModel) => IsEditMode && EditList.Contains(bookModel);

    /// <summary>
    /// Items that are edited or changed.
    /// </summary>
    public List<BookModel> EditList { get; } = new();

    /// <summary>
    /// Selection list contains items that were selected.
    /// </summary>
    public List<BookModel> SelectionList { get; } = new();

    /// <summary>
    /// Enter edit mode with selected item.
    /// </summary>
    /// <param name="bookModel">Book model.</param>
    public void Select(BookModel bookModel)
    {
        if (SelectionList.Contains(bookModel))
        {
            return;
        }

        SelectionList.Add(bookModel);
    }

    /// <summary>
    /// Unselect item from editable list.
    /// </summary>
    /// <param name="bookModel">Item to be unselected.</param>
    public void Unselect(BookModel bookModel)
    {
        SelectionList.Remove(bookModel);
    }

    /// <summary>
    /// Checks whether the item is selected.
    /// </summary>
    /// <param name="bookModel">Item to test.</param>
    /// <returns>True if selected.</returns>
    public bool IsSelected(BookModel bookModel)
    {
        return SelectionList.Contains(bookModel);
    }
}