using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Attendance.Views.Attendance;

public class AddReportModel : PageModel
{
    [BindProperty()]
    public int ProjectId { get; set; }
    
}