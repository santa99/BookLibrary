namespace Attendance.DataAccess.Dtos;

public class RoleDto
{
    public long Id { get; set; }

    public string Name { get; set; }

    public string Description { get; set; }

    public decimal CostCoefPerHour { get; set; }
}