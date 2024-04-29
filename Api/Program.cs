using Api;
using Api.Configuration;
using Api.Filters;
using Api.Mappers;
using Api.Middleware.Exceptions;
using Api.Middleware.Exceptions.Mappers;
using Contracts;
using DataAccess;
using DataAccess.Configuration;
using SimpleAuthentication;

var builder = WebApplication.CreateBuilder(args);

builder.Services.Configure<UserIdentityConfiguration>(builder.Configuration.GetSection("UserIdentity"));
builder.Services.Configure<BookLibraryDataSourceConfig>(builder.Configuration.GetSection("DataSource"));

builder.Services.AddSingleton<RequestModelValidationFilter>();
builder.Services.AddSingleton<BookStateMapper>();
builder.Services.AddSingleton<CustomAuthorizeFilter>();
builder.Services.AddSingleton<IReadersInfoRepository, ReadersInfoRepository>();
builder.Services.AddSingleton<IBookLibraryRepository, BookLibraryRepository>();
builder.Services.AddSingleton<IBookLibraryDao, BookLibraryDaoImpl>();
builder.Services.AddSingleton<IReadersInfoDao, ReadersInfoDaoImpl>();
builder.Services.AddSingleton<IBorrowBookCommand, BorrowBookCommand>();
builder.Services.AddSingleton<IResponseStatusCodeMapper, ResponseStatusCodeMapper>();
builder.Services.AddSingleton<IErrorResponseMapper, ErrorResponseMapper>();

builder.Services.AddSimpleAuthentication(builder.Configuration);

builder.Services.AddControllersWithViews();


var app = builder.Build();


app.UseMiddleware<ErrorHandlerMiddleware>();
app.UseAuthenticationAndAuthorization();
app.UseStaticFiles();

app.MapControllers();
app.Run();