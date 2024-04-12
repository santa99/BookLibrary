using Attendance.Business;
using Attendance.Models;
using Microsoft.AspNetCore.Mvc;

namespace Attendance.Controllers;

public class AttendanceController : Controller
{
    private readonly IInsertReportCommand _insertReportCommand;
    private readonly IGetReportsQuery _reportsQuery;

    public AttendanceController(IInsertReportCommand insertReportCommand, IGetReportsQuery reportsQuery)
    {
        _insertReportCommand = insertReportCommand;
        _reportsQuery = reportsQuery;
    }
    
    [Route("/")]
    public async Task<IActionResult> Index(CancellationToken cancellationToken)
    {
        var reports = await _reportsQuery.GetReportsAsync(cancellationToken);
        
        ViewData["appTitle"] = "Reports list";
        ViewData["reports"] = reports;

        return View(); //Views/Attendance/Index.cshtml
    }

    [HttpPost("attendance/reports/set")]
    public async Task SetReport([FromBody] AttendanceInsertRequest request, CancellationToken cancellationToken)
    {
        await _insertReportCommand.SetReportAsync(request.ProjectId, request.EmployeeId, request.PositionId, request.RoleId, request.TimeLoggedInMinutes, cancellationToken);
    }
    
    [HttpGet("attendance/projects")]
    public async Task<ViewResult> GetProjectWithPrize([FromRoute] int projectId, [FromQuery] DateTimeOffset from, DateTimeOffset to, CancellationToken cancellationToken)
    {
        var reportsByProjectAsync = await _reportsQuery.GetReportsByProjectAsync(projectId, from, to, cancellationToken);

        ViewData["appTitle"] = "Projects and prize";
        ViewData["reports"] = reportsByProjectAsync;

        return View();
    }
}