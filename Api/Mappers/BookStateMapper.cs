using Contracts;

namespace Api.Mappers;

/// <summary>
/// Book state mapper.
/// </summary>
public class BookStateMapper
{
    /// <summary>
    /// Maps <see cref="BookState"/> into the book state id.
    /// </summary>
    /// <param name="bookState"><see cref="BookState"/></param>
    /// <returns>Valid book state id.</returns>
    public int ToBookStateId(BookState bookState)
    {
        return (int)bookState;
    }

    /// <summary>
    /// Maps provided book state id into the <see cref="BookState"/>.
    /// </summary>
    /// <param name="bookStateId">Book state id.</param>
    /// <returns><see cref="BookState"/></returns>
    public BookState Map(int bookStateId)
    {
        return bookStateId switch
        {
            -1 => BookState.All,
            0 => BookState.Free,
            1 => BookState.Borrowed,
            _ => BookState.All
        };
    }
}