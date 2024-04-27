using Api;
using Api.Configuration;
using Contracts;
using DataAccess;
using SimpleAuthentication;

var builder = WebApplication.CreateBuilder(args);

builder.Services.Configure<UserIdentity>(builder.Configuration.GetSection("UserIdentity"));


builder.Services.AddSingleton<IReadersInfoRepository, ReadersInfoRepository>();
builder.Services.AddSingleton<IBookLibraryRepository, BookLibraryRepository>();
builder.Services.AddSingleton<LibraryDb>();
builder.Services.AddSingleton<ILibraryDb, LibraryDb>();
builder.Services.AddSingleton<IBookCommand, BookCommand>();

builder.Services.AddSimpleAuthentication(builder.Configuration);

builder.Services.AddControllersWithViews();


var app = builder.Build();

app.UseAuthenticationAndAuthorization();

app.MapControllers();
app.Run();