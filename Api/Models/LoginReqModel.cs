using System.ComponentModel.DataAnnotations;

namespace Api.Models;

/// <summary>
/// Represent data structure for login purpose.
/// </summary>
/// <param name="Username">User name.</param>
/// <param name="Password">Valid password.</param>
public record LoginReqModel(
    string Username,
    [DataType(DataType.Password)] 
    string Password
);