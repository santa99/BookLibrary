using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;

namespace Api.Models;

/// <summary>
/// Create request model for the book.
/// </summary>
/// <param name="BookId">Unique book id.</param>
/// <param name="Title">Name of the book.</param>
/// <param name="Author">Author of the book.</param>
public record UpdateBookReqModel(
    [FromRoute]
    [Required(ErrorMessage = "{0} can't be empty or null.")]
    [Range(1, int.MaxValue)]
    int BookId,
    [Required(ErrorMessage = "{0} can't be empty or null.")]
    [StringLength(15, ErrorMessage = "{0} should be {1} characters.")]
    [FromQuery]
    string Title,
    [Required(ErrorMessage = "{0} can't be empty or null.")]
    [FromQuery]
    string Author
);