using Api.Exceptions;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Api.Filters;

/// <summary>
/// Represent filter handling request model validations.
/// </summary>
public class RequestModelValidationFilter : IAsyncActionFilter
{
    /// <inheritdoc />
    public Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        if (context.ModelState.IsValid)
        {
            return next();
        }

        var errors = string
            .Join("\n", context.ModelState.Values
                .SelectMany(value => value.Errors)
                .Select(err => err.ErrorMessage)
            );

        throw new RequestValidationException(errors);
    }
}