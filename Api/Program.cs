using System.Reflection;
using Api;
using Api.Configuration;
using Api.Filters;
using Api.Logging;
using Api.Mappers;
using Api.Middleware.Exceptions;
using Api.Middleware.Exceptions.Mappers;
using Contracts;
using DataAccess;
using DataAccess.Configuration;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.OpenApi.Models;
using Serilog;
using SimpleAuthentication;

var builder = WebApplication.CreateBuilder(args);

var originPolicy = "originPolicy";

builder.Host.ConfigureLogging(loggingBuilder =>
{
    loggingBuilder.ClearProviders();
});
builder.Host.UseSerilog((context, configuration) =>
{
    configuration.ReadFrom.Configuration(context.Configuration);
});

builder.Services.Configure<UserIdentityConfiguration>(builder.Configuration.GetSection("UserIdentity"));
builder.Services.Configure<BookLibraryDataSourceConfig>(builder.Configuration.GetSection("DataSource"));


builder.Services.AddSingleton<IBookLibraryLoggerProvider, BookLibraryLoggerProvider>();
builder.Services.TryAddSingleton<IHttpContextAccessor, HttpContextAccessor>();
builder.Services.TryAddSingleton<IActionContextAccessor, ActionContextAccessor>();
builder.Services.AddSingleton<IHttpRequestLogger>(_ => builder.Services.BuildServiceProvider().GetRequiredService<IBookLibraryLoggerProvider>().InLogger);

builder.Services.AddSwaggerGen(options =>
{
    options.AddSecurityDefinition(CookieAuthenticationDefaults.AuthenticationScheme, new OpenApiSecurityScheme
    {
        Scheme = "cookies",
        Type = SecuritySchemeType.ApiKey,
        Name = "auth",
        In = ParameterLocation.Cookie,
        Description = "Please enter auth cookie read from login response headers."
    });
    options.SwaggerDoc("v1", new OpenApiInfo() {Title = "Book Library API Doc", Version = "v1"});
    options.IncludeXmlComments(Path.Combine(
        AppContext.BaseDirectory, 
        $"{Assembly.GetExecutingAssembly().GetName().Name}.xml")
    );
    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = CookieAuthenticationDefaults.AuthenticationScheme
                }
            },
            new string[]{}
        }
    });
});

builder.Services.AddCors(options =>
{
    options.AddPolicy(name: originPolicy,
        policy =>
        {
            policy.WithOrigins("https://localhost:7292");
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
        options.ExpireTimeSpan = TimeSpan.FromMinutes(60);
    });

builder.Services.AddSingleton<RequestModelValidationFilter>();
builder.Services.AddSingleton<BookStateMapper>();
builder.Services.AddSingleton<IReadersInfoRepository, ReadersInfoRepository>();
builder.Services.AddSingleton<IBookLibraryRepository, BookLibraryRepository>();
builder.Services.AddSingleton<IBookLibraryDao, BookLibraryDaoNewImpl>();
builder.Services.AddSingleton<IReadersInfoDao, ReadersInfoDaoImpl>();
builder.Services.AddSingleton<IBorrowBookCommand, BorrowBookCommand>();
builder.Services.AddSingleton<IResponseStatusCodeMapper, ResponseStatusCodeMapper>();
builder.Services.AddSingleton<IErrorResponseMapper, ErrorResponseMapper>();

builder.Services.AddControllersWithViews();


var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        var swaggerEndpointPath = $"swagger/v1/swagger.json";
        options.SwaggerEndpoint($"../{swaggerEndpointPath}", "v1");
    });
}

app.UseCors(originPolicy);

app.UseAuthentication();
app.UseAuthorization();

app.UseMiddleware<HttpTrafficLoggerMiddleware>();
app.UseMiddleware<ErrorHandlerMiddleware>();
app.UseStaticFiles();

app.MapControllers();
app.Run();