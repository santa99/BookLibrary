using Api.Configuration;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Options;

namespace Api.Filters;

public class CustomAuthorizeFilter : IAuthorizationFilter
{
    private readonly UserIdentityConfiguration _userIdentityConfigurationValue;

    public CustomAuthorizeFilter(IOptions<UserIdentityConfiguration> userIdentity)
    {
        _userIdentityConfigurationValue = userIdentity.Value;
    }

    public void OnAuthorization(AuthorizationFilterContext context)
    {
        if (!context.HttpContext.Request.Cookies.TryGetValue("user", out string? userVal))
        {
            context.Result = new ForbidResult();
        }
        
        if (userVal != _userIdentityConfigurationValue.User)
        {
            context.Result = new ForbidResult();
        }
        
    }
}