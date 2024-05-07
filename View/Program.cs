using View.Controllers;
using View.Services;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddSingleton<BooksController>();
builder.Services.AddScoped<BorrowState>();
builder.Services.AddScoped<PaginationState>();

builder.Services.AddHttpClient();
builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();

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