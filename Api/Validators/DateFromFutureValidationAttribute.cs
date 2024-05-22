using System.ComponentModel.DataAnnotations;

namespace Api.Validators;

/// <summary>
/// Validation attribute expecting the date not to be from the future.
/// </summary>
public class DateFromFutureValidationAttribute : ValidationAttribute
{
    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
        if (value == null)
        {
            return null;
        }
        
        var date = (DateTimeOffset)value;
        var nowDate = DateTimeOffset.UtcNow;
        if (date.Day > nowDate.Day || date.Month > nowDate.Month || date.Year > nowDate.Year)
        {
            return new ValidationResult(string.Format(ErrorMessage ?? "Date can't be from the future."));
        }

        return ValidationResult.Success;

    }
}