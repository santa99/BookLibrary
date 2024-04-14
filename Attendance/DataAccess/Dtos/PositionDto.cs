namespace Attendance.DataAccess.Dtos;

public class PositionDto
{
    public int Id { get; set; }

    public string Name { get; set; }

    public string Description { get; set; }

    public decimal CostCoefPerHour { get; set; }
}