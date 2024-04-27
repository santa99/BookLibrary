using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

public class HomeController : Controller
{
    [Route("/")]
    public async Task<IActionResult> Index()
    {
        var user = Request.Cookies["user"];
        if (user != null)
        {
            ViewBag.User = user;
        }
        return View();
    }
}