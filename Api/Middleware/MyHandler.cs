﻿namespace Api.Middleware;

public class MyHandler
{
    private readonly RequestDelegate _next;
    private readonly ILogger<MyHandler> _logger;

    public MyHandler(RequestDelegate next, ILogger<MyHandler> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task Invoke(HttpContext context)
    {
        _logger.LogInformation($"Received request: {context.Request.Path}");

        if (context.User.Identity.IsAuthenticated)
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