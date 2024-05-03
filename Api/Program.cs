using Api;
using Api.Configuration;
using Api.Filters;
using Api.Mappers;
using Api.Middleware.Exceptions;
using Api.Middleware.Exceptions.Mappers;
using Contracts;
using DataAccess;
using DataAccess.Configuration;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using SimpleAuthentication;

var builder = WebApplication.CreateBuilder(args);

builder.Services.Configure<UserIdentityConfiguration>(builder.Configuration.GetSection("UserIdentity"));
builder.Services.Configure<BookLibraryDataSourceConfig>(builder.Configuration.GetSection("DataSource"));

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/account/login/";
        options.LogoutPath = "/account/logout/";
        options.Cookie.Name = "usr";
        options.SlidingExpiration = true;
        options.ExpireTimeSpan = TimeSpan.FromMinutes(5);
    });

builder.Services.AddAuthorization(options =>
{
    options.DefaultPolicy = new AuthorizationPolicyBuilder()
        .AddAuthenticationSchemes(CookieAuthenticationDefaults.AuthenticationScheme)
        .RequireAuthenticatedUser()
        .Build();
});

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

app.UseCookiePolicy(new CookiePolicyOptions
{
    MinimumSameSitePolicy = SameSiteMode.Strict,
});

app.UseAuthentication();
app.UseAuthorization();

app.UseMiddleware<ErrorHandlerMiddleware>();
app.UseAuthenticationAndAuthorization();
app.UseStaticFiles();

app.MapControllers();
app.Run();