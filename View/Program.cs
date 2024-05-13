using System.Net.Http.Headers;
using Api.Middleware;
using View.Controllers;
using View.Services;

var builder = WebApplication.CreateBuilder(args);

var MyAllowSpecificOrigins = "_myAllowSpecificOrigins";


builder.Services.AddSingleton<BooksController>();
builder.Services.AddScoped<BorrowState>();
builder.Services.AddScoped<PaginationState>();
builder.Services.AddScoped<EditState>();

// Services for VIEW communication.
builder.Services.AddTransient<ReadersInfoService>();
builder.Services.AddTransient<BooksService>();
builder.Services.AddSingleton<LoginService>();

builder.Services.AddHttpClient();
builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();

// builder.Services.AddScoped<SessionIdHandler>();
// builder.Services.ConfigureAll<HttpClientFactoryOptions>(options =>
// {
//     options.HttpMessageHandlerBuilderActions.Add(builder =>
//     {
//         builder.AdditionalHandlers.Add(builder.Services.GetRequiredService<SessionIdHandler>());
//     });
// });

builder.Services.AddCors(options =>
{
    options.AddPolicy(name: MyAllowSpecificOrigins,
        policy =>
        {
            policy.WithOrigins("https://localhost:7227/");
            policy.AllowCredentials();
            policy.AllowAnyHeader();
            policy.AllowAnyMethod();
            policy.WithHeaders();
        });
});

var app = builder.Build();

// Routing for controllers 
app.MapControllerRoute("default", "{controller=Books}/{action=Index}/{id?}");

//wwwroot mapping
app.UseStaticFiles();
app.UseRouting();

app.UseCors(MyAllowSpecificOrigins);

// Razor pages
app.MapRazorPages();
app.MapBlazorHub();

app.UseMiddleware<LoginMiddleware>();

// Initial page
app.MapFallbackToPage("/_Host");

app.Run();