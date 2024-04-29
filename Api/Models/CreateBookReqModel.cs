using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;

namespace Api.Models;

/// <summary>
/// Create request model for the book.
/// </summary>
/// <param name="Title">Name of the book.</param>
/// <param name="Author">Author of the book.</param>
public record CreateBookReqModel(
    [Required(ErrorMessage = "{0} can't be empty or null.")]
    [StringLength(15, ErrorMessage = "{0} should be {1} characters.")]
    [FromQuery]
    string Title,
    [Required(ErrorMessage = "{0} can't be empty or null.")]
    [FromQuery]
    string Author
);