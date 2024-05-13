using System.Security.Claims;
using Api.Configuration;
using Api.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Primitives;
using Microsoft.Net.Http.Headers;

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

    private void WriteAuthCookie()
    {
        if (Request.Cookies.TryGetValue("usr", out var val) && val != null)
        {
            CookieOptions option = new CookieOptions
            {
                Expires = DateTime.Now.AddMilliseconds(10)
            };
            Response.Cookies.Append("usr", val, option);
            
            CookieHeaderValue cookieHeaderValue = new CookieHeaderValue("usr", val);
            Response.Headers.Cookie.Append(cookieHeaderValue.ToString());

            Response.Headers.SetCookie = new StringValues(cookieHeaderValue.ToString());
        }
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
            WriteAuthCookie();
            
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
            /*if (HttpContext.User.Identity is { IsAuthenticated: true })
            {
                return LocalRedirect(returnUrl);
            }*/

            var user = await AuthenticateUser(model);

            if (user == null)
            {
                ModelState.AddModelError(string.Empty, "Invalid login attempt.");

                return BadRequest(new AccountResponse(false, "Invalid credentials."));
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

            return Ok(new AccountResponse(true, "Signed in successfully"));
        }

        return BadRequest(new AccountResponse(false, "Invalid data."));
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
    
    [Authorize]
    [HttpGet("/account/user")]
    public Task<IActionResult> GetUser()
    {
        var enumerable = HttpContext.User.Claims.Select(claim => new UserClaim(claim.Type, claim.Value));
        var userClaims =  enumerable.ToList();
        
        return Task.FromResult<IActionResult>(Ok(userClaims));
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

    private record AccountResponse(bool IsSuccess, string Message);

    private record UserClaim(string Type, string Value);
}