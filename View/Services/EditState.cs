namespace View.Services;

public class EditState
{
    /// <summary>
    /// Checks whether in edit mode.
    /// </summary>
    public bool IsEditMode { get; private set; }

    public void EnterEditMode()
    {
        IsEditMode = true;
    }

    public void ExitEditMode()
    {
        IsEditMode = false;
    }

    public async Task EditBook(int itemId)
    {
        
    }

    public async Task RemoveBook(int itemId)
    {
        
    }
}