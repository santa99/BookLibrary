using System.ComponentModel.DataAnnotations;
using Api.Configuration;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace Api.Controllers;

public class AccountController : Controller
{
    private readonly IOptions<UserIdentity> _userIdentity;

    public AccountController(IOptions<UserIdentity> userIdentity)
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
            // Generujeme cookie
            var cookieOptions = new CookieOptions();
            cookieOptions.Expires = DateTime.Now.AddMinutes(5);
            Response.Cookies.Append("userId", "99", cookieOptions);
            Response.Cookies.Append("user",userIdentityValue.User ,cookieOptions);

            return LocalRedirect(returnUrl);
        }

        ModelState.AddModelError("", "Incorrect name or password.");

        return View(model);
    }

    public record LoginViewModel(string username, [DataType(DataType.Password)] string password);
}