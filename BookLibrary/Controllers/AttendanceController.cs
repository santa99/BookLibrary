using Attendance.Business;
using Attendance.Models;
using Microsoft.AspNetCore.Mvc;

namespace Attendance.Controllers;

public class AttendanceController : Controller
{
    private readonly IInsertReportCommand _insertReportCommand;
    private readonly IGetReportsQuery _reportsQuery;
    private readonly IEmployeesQuery _employeesQuery;
    private readonly IPositionsQuery _positionsQuery;
    private readonly IProjectsQuery _projectsQuery;

    public AttendanceController(IInsertReportCommand insertReportCommand, IGetReportsQuery reportsQuery,
        IEmployeesQuery employeesQuery, IPositionsQuery positionsQuery, IProjectsQuery projectsQuery)
    {
        _insertReportCommand = insertReportCommand;
        _reportsQuery = reportsQuery;
        _employeesQuery = employeesQuery;
        _positionsQuery = positionsQuery;
        _projectsQuery = projectsQuery;
    }

    [Route("/")]
    public async Task<IActionResult> Index(CancellationToken cancellationToken)
    {
        var reports = await _reportsQuery.GetReportsAsync(cancellationToken);

        ViewData["appTitle"] = "Reports list";
        ViewData["reports"] = reports;

        return View(); //Views/BookLibrary/Index.cshtml
    }

    [Route("attendance/reports/add")]
    public async Task<IActionResult> AddReport(CancellationToken cancellationToken)
    {
        if (Request.Method == HttpMethod.Post.ToString())
        {
            await SetReportAction(new AttendanceInsertRequest()
            {
                ProjectId = int.Parse(Request.Form["projectId"]),
                EmployeeId = int.Parse(Request.Form["employeeId"]),
                PositionId = int.Parse(Request.Form["positionId"]),
                TimeLoggedInMinutes = int.Parse(Request.Form["timeLogged"])
            }, cancellationToken);

            Response.Redirect("/");
        }

        var projects = await _projectsQuery.GetAllEmployees(cancellationToken);
        var employees = await _employeesQuery.GetAllEmployees(cancellationToken);
        var positions = await _positionsQuery.GetAllPositions(cancellationToken);
        ViewData["projects"] = projects;
        ViewData["employees"] = employees;
        ViewData["positions"] = positions;

        return View();
    }

    [HttpPost("attendance/reports/insert")]
    public async Task SetReportAction([FromBody] AttendanceInsertRequest? request, CancellationToken cancellationToken)
    {
        //TODO: data validation
        await _insertReportCommand.SetReportAsync(request.ProjectId, request.EmployeeId, request.PositionId,
            request.TimeLoggedInMinutes, cancellationToken);
    }

    [HttpGet("attendance/projects")]
    public async Task<ViewResult> GetProjectWithPrize(
        [FromQuery] int projectId, [FromQuery] DateTimeOffset? from, DateTimeOffset? to,
        CancellationToken cancellationToken)
    {
        from ??= DateTimeOffset.FromUnixTimeMilliseconds(100000);
        to ??= DateTimeOffset.Now.Add(TimeSpan.FromDays(1));
        
        var reportsByProjectAsync =
            await _reportsQuery.GetReportsByProjectAsync(projectId, from!, to, cancellationToken);

        ViewData["appTitle"] = "Projects and prize";
        ViewData["reports"] = reportsByProjectAsync;

        return View();
    }
}