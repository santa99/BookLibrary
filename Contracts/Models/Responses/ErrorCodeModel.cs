namespace Contracts.Models.Responses;

/// <summary>
///    Class <see cref="ErrorCodeModel"/> represents model used for reporting errors.
/// </summary>
public record ErrorCodeModel(
    int Id,
    string? Message,
    string? ClientMessage
);

