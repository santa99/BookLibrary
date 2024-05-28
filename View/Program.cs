using Api.Middleware;
using Contracts.Models;
using View.Data;
using View.Services;

var builder = WebApplication.CreateBuilder(args);

var MyAllowSpecificOrigins = "_myAllowSpecificOrigins";


builder.Services.AddScoped<BorrowState>();
builder.Services.AddScoped<PaginationState>();

// Services for VIEW communication.
builder.Services.AddSingleton<ReadersInfoService>();
builder.Services.AddSingleton<LoginService>();
builder.Services.AddSingleton<BooksService>();
builder.Services.AddTransient<BookViewService>();
builder.Services.AddTransient<IListViewService<BookModel>, BookViewService>();
builder.Services.AddTransient<IBorrowReturnViewService, BorrowReturnViewService>();
builder.Services.AddTransient<IEditableListViewService<BookModel>, EditableBookViewService>();


builder.Services.AddHttpClient();
builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();

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