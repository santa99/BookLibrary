using System.ComponentModel.DataAnnotations;

namespace Api.Models;

/// <summary>
/// Represent data structure for login purpose.
/// </summary>
/// <param name="username">User name.</param>
/// <param name="password">Valid password.</param>
public record LoginViewModel(string username, [DataType(DataType.Password)] string password);