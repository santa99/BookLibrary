using Attendance;
using Attendance.Business;
using Attendance.DataAccess;
using Attendance.DataAccess.Repositories;
using Attendance.DataAccess.Utilities;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllersWithViews();

var configurationSection = builder.Configuration.GetSection("UserIdentity");
configurationSection.Bind();
builder.Services.Configure<UserIdentity>((IConfiguration) section1);
builder.Configuration.AddConfigurationAnd();

builder.Services.AddConfigOptionsAndBind(builder.Configuration,
    "ConnectionStrings", out DatabaseConnections instance);

builder.Services.AddSingleton<IInsertReportCommand, InsertReportCommand>();
builder.Services.AddSingleton<IGetReportsQuery, GetReportsQuery>();
builder.Services.AddSingleton<IEmployeesQuery, EmployeesQuery>();
builder.Services.AddSingleton<IPositionsQuery, PositionsQuery>();
builder.Services.AddSingleton<IProjectsQuery, ProjectsQuery>();
builder.Services.AddSingleton<IReportsRepository, ReportsRepository>();
builder.Services.AddSingleton<IDatabaseConnectionProvider, DatabaseConnectionProvider>();

var app = builder.Build();


app.UseStaticFiles();
app.MapControllers();

app.Run();