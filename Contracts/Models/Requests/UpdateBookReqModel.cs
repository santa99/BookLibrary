using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;

namespace Contracts.Models.Requests;

public record UpdateBookReqModel(
    [FromRoute]
    [Required(ErrorMessage = "{0} can't be empty or null.")]
    [Range(1, int.MaxValue)]
    int BookId,
    [Required(ErrorMessage = "{0} can't be empty or null.")]
    [StringLength(15, ErrorMessage = "{0} should be {1} characters max.")]
    [FromQuery]
    string Title,
    [Required(ErrorMessage = "{0} can't be empty or null.")]
    [FromQuery]
    string Author
);