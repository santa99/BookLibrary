using System.ComponentModel.DataAnnotations;
using Api.Validators;

namespace Api.Models;

/// <summary>
/// Create request model for the book borrowing.
/// </summary>
/// <param name="ReadersCardId">Book id to be borrowed.</param>
/// <param name="ReadersCardId">Readers card id.</param>
/// <param name="From">Date of the borrow.</param>
public record CreateBorrowReqModel(
    [Required(ErrorMessage = "{0} can't be empty or null.")]
    [Range(1, int.MaxValue, ErrorMessage = "{0} should be between ${1} and ${2}.")]
    int BookId,
    [Required(ErrorMessage = "{0} can't be empty or null.")]
    int ReadersCardId,
    [Required(ErrorMessage = "{0} can't be empty or null.")]
    [DateFromFutureValidation]
    DateTimeOffset From
);