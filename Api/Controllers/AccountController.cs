using Api.Configuration;
using Api.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace Api.Controllers;

/// <summary>
/// Account controller handles login/logout.
/// </summary>
public class AccountController : Controller
{
    private readonly IOptions<UserIdentityConfiguration> _userIdentity;

    public AccountController(IOptions<UserIdentityConfiguration> userIdentity)
    {
        _userIdentity = userIdentity;
    }

    [HttpGet("/account/login")]
    public Task<IActionResult> Login(string returnUrl = "/")
    {
        var authenticatedUser = Request.Cookies["user"];
        if (authenticatedUser != null)
        {
            return Task.FromResult<IActionResult>(LocalRedirect(returnUrl));
        }

        return Task.FromResult<IActionResult>(View());
    }

    [HttpPost("/account/login")]
    public Task<IActionResult> Login([FromForm] LoginReqModel model, string returnUrl = "/")
    {
        var userIdentityValue = _userIdentity.Value;
        if (model.Username == userIdentityValue?.User && model.Password == userIdentityValue?.Password)
        {
            var cookieOptions = new CookieOptions
            {
                Expires = DateTime.Now.AddMinutes(10)
            };
            Response.Cookies.Append("user", userIdentityValue.User, cookieOptions);

            return Task.FromResult<IActionResult>(LocalRedirect(returnUrl));
        }

        ViewBag.TextMessage = "Incorrect name or password.";

        return Task.FromResult<IActionResult>(View(model));
    }


    [HttpGet("/account/logout")]
    public Task<IActionResult> Logout(string returnUrl)
    {
        Response.Cookies.Delete("user");

        return Task.FromResult<IActionResult>(LocalRedirect(returnUrl));
    }
}