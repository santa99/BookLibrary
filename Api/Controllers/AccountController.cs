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
[Authorize]
public class AccountController : Controller
{
    private readonly IOptions<UserIdentityConfiguration> _userIdentity;

    public AccountController(IOptions<UserIdentityConfiguration> userIdentity)
    {
        _userIdentity = userIdentity;
    }

    [AllowAnonymous]
    [HttpGet("/account/login")]
    public Task<IActionResult> Login(string returnUrl = "/")
    {
        if (!ModelState.IsValid)
        {
            return Task.FromResult<IActionResult>(View());
        }

        if (HttpContext.User.Identity is { IsAuthenticated: true })
        {
            return Task.FromResult<IActionResult>(LocalRedirect(returnUrl));
        }

        return Task.FromResult<IActionResult>(View());
    }

    [AllowAnonymous]
    [HttpPost("/account/login")]
    public async Task<IActionResult> Login([FromForm] LoginReqModel model, string returnUrl = "/")
    {
        if (ModelState.IsValid)
        {
            var user = await AuthenticateUser(model);

            if (user == null)
            {
                ModelState.AddModelError(string.Empty, "Invalid login attempt.");

                return View(model);
            }

            var claims = new List<Claim>()
            {
                new(ClaimTypes.Name, user.Name),
                new(ClaimTypes.Role, "Admin"),
            };

            var claimsIdentity = new ClaimsIdentity(
                claims, CookieAuthenticationDefaults.AuthenticationScheme
            );

            var authenticationProperties = new AuthenticationProperties()
            {

            };

            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(claimsIdentity),
                authenticationProperties);

            return LocalRedirect(returnUrl);
        }

        return View(model);
    }

    [AllowAnonymous]
    [HttpGet("/account/logout")]
    public async Task<IActionResult> Logout(string? returnUrl = "/")
    {
        // Clear the existing external cookie
        await HttpContext.SignOutAsync(
            CookieAuthenticationDefaults.AuthenticationScheme);

        return LocalRedirect(returnUrl);
    }
    
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
    
}