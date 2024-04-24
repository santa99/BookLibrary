using Contracts;
using DataAccess;
using SimpleAuthentication;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSingleton<IReadersInfoRepository, ReadersInfoRepository>();
builder.Services.AddSingleton<IBookLibraryRepository, BookLibraryRepository>();
builder.Services.AddSingleton<XmlDatabase>();
builder.Services.AddSingleton<ILibraryDb, XmlDatabase>();

builder.Services.AddSimpleAuthentication(builder.Configuration);

builder.Services.AddControllersWithViews();


var app = builder.Build();

app.UseAuthenticationAndAuthorization();

app.MapControllers();
app.Run();