namespace Attendance.DataAccess.Dtos
{
    public class ReportDto
    {
        public long Id { get; set; }
        /*public int EmployeeId { get; set; }
        public int PositionId { get; set; }
        public int RoleId { get; set; }
        public short TimeLoggedInMinutes { get; set; }
        public DateTimeOffset CDate { get; set; }*/
        
        
        public string Project { get; set; }
        public string Person { get; set; }
        public string WorkedAs { get; set; }
        public string WorkedAsDesc { get; set; }
        public int WorkedAsCost { get; set; }
        public int HowLong { get; set; }
        public DateTimeOffset WhenItHappend { get; set; }
        
    }
}