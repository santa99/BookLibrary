using System.Reflection;
using Api;
using Api.Configuration;
using Api.Filters;
using Api.Mappers;
using Api.Middleware;
using Api.Middleware.Exceptions;
using Api.Middleware.Exceptions.Mappers;
using Contracts;
using DataAccess;
using DataAccess.Configuration;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Http;
using Microsoft.OpenApi.Models;
using SimpleAuthentication;
using View;
using View.Services;

var builder = WebApplication.CreateBuilder(args);

var MyAllowSpecificOrigins = "_myAllowSpecificOrigins";

builder.Services.Configure<UserIdentityConfiguration>(builder.Configuration.GetSection("UserIdentity"));
builder.Services.Configure<BookLibraryDataSourceConfig>(builder.Configuration.GetSection("DataSource"));

builder.Services.TryAddSingleton<IHttpContextAccessor, HttpContextAccessor>();
builder.Services.TryAddSingleton<IActionContextAccessor, ActionContextAccessor>();

builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo() {Title = "Book Library API Doc", Version = "v1"});
    options.IncludeXmlComments(Path.Combine(
        AppContext.BaseDirectory, 
        $"{Assembly.GetExecutingAssembly().GetName().Name}.xml")
    );
});

builder.Services.AddCors(options =>
{
    options.AddPolicy(name: MyAllowSpecificOrigins,
        policy =>
        {
            policy.WithOrigins("https://localhost:7292", "https://localhost:7292/readers");
            policy.AllowCredentials();
            policy.AllowAnyHeader();
            policy.AllowAnyMethod();
            policy.WithHeaders();
            });
});

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/account/login/";
        options.LogoutPath = "/account/logout/";
        options.SlidingExpiration = true;
        options.Cookie.Name = "auth";
        options.Cookie.SameSite = SameSiteMode.None;
        options.Cookie.HttpOnly = false;
        options.ExpireTimeSpan = TimeSpan.FromMinutes(5);
    });

// builder.Services.AddAuthorization(options =>
// {
//     options.DefaultPolicy = new AuthorizationPolicyBuilder()
//         .AddAuthenticationSchemes(CookieAuthenticationDefaults.AuthenticationScheme)
//         .RequireAuthenticatedUser()
//         .Build();
// });

// builder.Services.AddSession(options =>
// {
//     options.Cookie.Name = "auth";
//     options.IdleTimeout = TimeSpan.FromSeconds(10);
//     options.Cookie.IsEssential = true;
// });

builder.Services.AddSingleton<RequestModelValidationFilter>();
builder.Services.AddSingleton<BookStateMapper>();
builder.Services.AddSingleton<IReadersInfoRepository, ReadersInfoRepository>();
builder.Services.AddSingleton<IBookLibraryRepository, BookLibraryRepository>();
builder.Services.AddSingleton<IBookLibraryDao, BookLibraryDaoImpl>();
builder.Services.AddSingleton<IReadersInfoDao, ReadersInfoDaoImpl>();
builder.Services.AddSingleton<IBorrowBookCommand, BorrowBookCommand>();
builder.Services.AddSingleton<IResponseStatusCodeMapper, ResponseStatusCodeMapper>();
builder.Services.AddSingleton<IErrorResponseMapper, ErrorResponseMapper>();

builder.Services.AddControllersWithViews();


var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI(options =>
{
    var swaggerEndpointPath = $"swagger/v1/swagger.json";
    options.SwaggerEndpoint($"../{swaggerEndpointPath}", "v1");
}); 

// app.UseCookiePolicy(new CookiePolicyOptions
// {
//     MinimumSameSitePolicy = SameSiteMode.None,
// });


app.UseCors(MyAllowSpecificOrigins);
// app.UseSession();

app.UseAuthentication();
app.UseAuthorization();

app.UseMiddleware<ErrorHandlerMiddleware>();
app.UseAuthenticationAndAuthorization();
app.UseStaticFiles();

app.MapControllers();
app.Run();