using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SimpleAuthentication.JwtBearer;

namespace Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class IdentityController : ControllerBase
{

    [HttpGet]
    [AllowAnonymous]
    [Route("api/loginpage")]
    public async Task<IActionResult> LoginPage()
    {
        return Ok("Login page");
    }
    
    [HttpPost]
    [AllowAnonymous]
    [Route("api/loginreq")]
    public async Task<IActionResult> Login(LoginRequest request, [FromServices] IJwtBearerService jwtBearerService)
    {
        if (request.Username == "admin" && request.Password == "admin")
        {
            var token = jwtBearerService.CreateToken("test");
            
            return Ok(new LoginResponse(token));
        }

        return Forbid();
    }

    public record LoginRequest(string Username, string Password);
    public record LoginResponse(string token);
}