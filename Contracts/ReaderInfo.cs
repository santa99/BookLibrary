namespace Contracts;

/// <summary>
/// Class <see cref="ReaderInfo"/> represents data transfer object for reader.
/// </summary>
public class ReaderInfo
{
    /// <summary>
    /// Readers card id.
    /// </summary>
    public int ReaderCardId { get; set; }

    /// <summary>
    /// Readers first name.
    /// </summary>
    public string FirstName { get; set; }

    /// <summary>
    /// Readers last name.
    /// </summary>
    public string LastName { get; set; }
}