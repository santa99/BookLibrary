namespace Attendance.DataAccess.Dtos
{
    /// <summary>
    /// Complete report including all parameters.
    /// </summary>
    public class ReportDto
    {
        /// <summary>
        /// Unique report id.
        /// </summary>
        public long Id { get; set; }
        
        /// <summary>
        /// Project name
        /// </summary>
        public string Project { get; set; }
        
        /// <summary>
        /// Project id. 
        /// </summary>
        public int ProjectId { get; set; }

        /// <summary>
        /// Employee Name
        /// </summary>
        public string Employee { get; set; }

        /// <summary>
        /// Employee id.
        /// </summary>
        public int EmployeeId { get; set; }

        /// <summary>
        /// Position name.
        /// </summary>
        public string PositionName { get; set; }

        /// <summary>
        /// Position id.
        /// </summary>
        public int PositionId { get; set; }

        /// <summary>
        /// Position description.
        /// </summary>
        public string PositionDesc { get; set; }

        /// <summary>
        /// Cost per hour.
        /// </summary>
        public int CostPerHour { get; set; }

        /// <summary>
        /// Time logged.
        /// </summary>
        public int LogTimeMinutes { get; set; }

        /// <summary>
        /// Log date.
        /// </summary>
        public DateTimeOffset LogDate { get; set; }
    }
}