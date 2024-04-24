using Attendance.DataAccess.Dtos;

namespace Attendance.Models;

public class AttendanceReportsResponse
{
    public List<ReportDto> Reports { get; set; }
}