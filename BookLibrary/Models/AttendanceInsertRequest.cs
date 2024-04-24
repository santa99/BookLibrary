namespace Attendance.Models;

public class AttendanceInsertRequest
{
    public int ProjectId { get; set; }
    public int EmployeeId { get; set; }
    public int PositionId { get; set; }
    public int TimeLoggedInMinutes { get; set; }
}