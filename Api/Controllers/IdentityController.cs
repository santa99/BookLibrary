using Microsoft.AspNetCore.Mvc;
using SimpleAuthentication.JwtBearer;

namespace Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class IdentityController : ControllerBase
{
    
    [HttpPost]
    public LoginResponse Login(LoginRequest request, [FromServices] IJwtBearerService jwtBearerService)
    {
        var token = jwtBearerService.CreateToken("test");
        return new(token);
    }

    public record LoginRequest(string Username, string Password);
    public record LoginResponse(string token);
}