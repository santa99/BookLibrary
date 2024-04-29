
namespace Api.Configuration;

/// <summary>
/// Class <see cref="UserIdentityConfiguration"/> represents model for user identity to
/// properly authorize user against book library.
/// </summary>
public class UserIdentityConfiguration
{
    /// <summary>
    /// User able editing library.
    /// </summary>
    public string User { get; set; }
    /// <summary>
    /// Password.
    /// </summary>
    public string Password { get; set; }
}