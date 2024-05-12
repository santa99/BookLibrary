using System.Net.Http.Headers;
using View.Controllers;
using View.Services;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddSingleton<BooksController>();
builder.Services.AddScoped<BorrowState>();
builder.Services.AddScoped<PaginationState>();
builder.Services.AddScoped<EditState>();

// Services for VIEW communication.
builder.Services.AddTransient<ReadersInfoService>();
builder.Services.AddTransient<BooksService>();
builder.Services.AddTransient<LoginService>();

builder.Services.AddHttpClient();
builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();

builder.Services.AddHttpClient("BookLibrary", httpClient =>
{
    httpClient.BaseAddress = new Uri("https://localhost:7227");
    httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
});

var app = builder.Build();

// Routing for controllers 
app.MapControllerRoute("default", "{controller=Books}/{action=Index}/{id?}");

//wwwroot mapping
app.UseStaticFiles();
app.UseRouting();

// Razor pages
app.MapRazorPages();
app.MapBlazorHub();

// Initial page
app.MapFallbackToPage("/_Host");

app.Run();