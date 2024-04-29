using Api.Configuration;
using Api.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace Api.Controllers;

public class AccountController : Controller
{
    private readonly IOptions<UserIdentityConfiguration> _userIdentity;

    public AccountController(IOptions<UserIdentityConfiguration> userIdentity)
    {
        _userIdentity = userIdentity;
    }

    [HttpGet("/account/login")]
    public async Task<IActionResult> Login(string returnUrl = "/")
    {
        var authenticatedUser = Request.Cookies["user"];
        if (authenticatedUser != null)
        {
            return LocalRedirect(returnUrl);
        }

        return View();
    }

    [HttpPost("/account/login")]
    public async Task<IActionResult> Login([FromForm] LoginViewModel model, string returnUrl = "/")
    {
        var userIdentityValue = _userIdentity.Value;
        if (model.username == userIdentityValue?.User && model.password == userIdentityValue?.Password)
        {
            var cookieOptions = new CookieOptions
            {
                Expires = DateTime.Now.AddMinutes(10)
            };
            Response.Cookies.Append("user", userIdentityValue.User, cookieOptions);

            return LocalRedirect(returnUrl);
        }

        ModelState.AddModelError("", "Incorrect name or password.");

        return View(model);
    }


    [HttpGet("/account/logout")]
    public Task<IActionResult> Logout(string returnUrl)
    {
        Response.Cookies.Delete("user");

        return Task.FromResult<IActionResult>(LocalRedirect(returnUrl));
    }
}