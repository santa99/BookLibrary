using System.Net.Mime;
using System.Security.Claims;
using Api.Configuration;
using Api.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace Api.Controllers;

/// <summary>
/// Account controller handles login/logout.
/// </summary>
[Authorize(AuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme)]
public class AccountController : Controller
{
    private readonly IOptions<UserIdentityConfiguration> _userIdentity;

    /// <inheritdoc />
    public AccountController(IOptions<UserIdentityConfiguration> userIdentity)
    {
        _userIdentity = userIdentity;
    }

    /// <summary>
    /// Call this method to send sign in.
    /// </summary>
    /// <param name="model">Credentials.</param>
    /// <param name="returnUrl">Return url</param>
    /// <returns></returns>
    [AllowAnonymous]
    [HttpPost("/account/login")]
    [ProducesResponseType(typeof(AccountResponse),StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(AccountResponse),StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(AccountResponse),StatusCodes.Status401Unauthorized)]
    [Produces( MediaTypeNames.Application.Json )]
    public async Task<IActionResult> Login([FromForm] LoginReqModel model, string returnUrl = "/")
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(new AccountResponse(false, "Invalid data."));
        }

        var user = await AuthenticateUser(model);

        if (user == null)
        {
            ModelState.AddModelError(string.Empty, "Invalid login attempt.");

            return Unauthorized(new AccountResponse(false, "Invalid credentials."));
        }

        var claims = new List<Claim>()
        {
            new(ClaimTypes.Name, user.Name),
            new(ClaimTypes.Role, "Admin"),
        };

        var claimsIdentity = new ClaimsIdentity(
            claims, CookieAuthenticationDefaults.AuthenticationScheme
        );

        var authenticationProperties = new AuthenticationProperties
        {
            IsPersistent = true,
            AllowRefresh = true,
            ExpiresUtc = DateTimeOffset.UtcNow.AddMinutes(10),
        };

        await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme,
            new ClaimsPrincipal(claimsIdentity),
            authenticationProperties);

        return Ok(new AccountResponse(true, "Signed in successfully."));
    }

    /// <summary>
    /// Log out current user.
    /// </summary>
    /// <param name="returnUrl"></param>
    /// <returns></returns>
    [AllowAnonymous]
    [HttpGet("/account/logout")]
    public async Task<IActionResult> Logout(string? returnUrl = "/")
    {
        await HttpContext.SignOutAsync(
            CookieAuthenticationDefaults.AuthenticationScheme);
        return Ok(returnUrl);        
    }

    /// <summary>
    /// Get current claims of currently signed in user.
    /// </summary>
    /// <returns>User claims.</returns>
    [HttpGet("/account/user")]
    [ProducesResponseType(typeof(List<UserClaim>),StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(List<UserClaim>),StatusCodes.Status405MethodNotAllowed)]
    public Task<List<UserClaim>> GetUser()
    {
        var enumerable = HttpContext.User.Claims.Select(claim => new UserClaim(claim.Type, claim.Value));
        var userClaims = enumerable.ToList();

        return Task.FromResult(userClaims);
    }

    /// <summary>
    /// This method should be replaced by the access to user accounts.
    /// </summary>
    /// <param name="login">Login model.</param>
    /// <returns>User model returned when provided parameters are properly authenticated.</returns>
    private Task<UserModel?> AuthenticateUser(LoginReqModel login)
    {
        UserModel user = null;
        if (login.Username == _userIdentity.Value.User &&
            login.Password == _userIdentity.Value.Password)
        {
            user = new UserModel("Marek");
        }

        return Task.FromResult(user);
    }

    /// <summary>
    /// User model record.
    /// </summary>
    /// <param name="Name">Display name</param>
    private record UserModel(string Name);
    
    /// <summary>
    /// Account response model.
    /// </summary>
    /// <param name="IsSuccess">True when user properly authentifacated</param>
    /// <param name="Message">Specific message.</param>
    private record AccountResponse(bool IsSuccess, string Message);

    /// <summary>
    /// User claim model.
    /// </summary>
    /// <param name="Type">Type of claim like Name, Fullname any other key.</param>
    /// <param name="Value">Value of claim.</param>
    public record UserClaim(string Type, string Value);
}