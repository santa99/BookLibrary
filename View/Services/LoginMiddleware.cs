namespace Api.Middleware;

public class LoginMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<LoginMiddleware> _logger;

    public LoginMiddleware(RequestDelegate next, ILogger<LoginMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task Invoke(HttpContext context)
    {
        _logger.LogInformation($"Received request: {context.Request.Path}");

        var containsKey = context.Request.Cookies.ContainsKey("auth");

        if (containsKey || context.User.Identity.IsAuthenticated)
        {
            var x = "AYES";
        }
        
        var firstOrDefault =
            context.Request.Headers.Cookie.FirstOrDefault(x => x.StartsWith("auth="));
        if (firstOrDefault != null)
        {
            var x = firstOrDefault;
        }

        await _next(context);
    }
}