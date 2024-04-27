using Api.Configuration;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Options;

namespace Api.Filters;

public class CustomAuthorizeFilter : IAuthorizationFilter
{
    private readonly UserIdentity _userIdentityValue;

    public CustomAuthorizeFilter(IOptions<UserIdentity> userIdentiy)
    {
        _userIdentityValue = userIdentiy.Value;
    }

    public void OnAuthorization(AuthorizationFilterContext context)
    {
        if (!context.HttpContext.Request.Cookies.TryGetValue("user", out string? userVal))
        {
            context.Result = new ForbidResult();
        }
        
        if (userVal != _userIdentityValue.User)
        {
            context.Result = new ForbidResult();
        }
        
    }
}