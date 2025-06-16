using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using ReportService.Models;
using ReportService.Repositories;
using ReportService.Services.ReportFormatter;
using ReportService.Services.Salary;
using ReportService.Utils;

namespace ReportService.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ReportController(IEmployeeRepository repository, ISalaryService salaryService, IReportFormatter  reportFormatter)
    : ControllerBase
{
    [HttpGet("{year}/{month}")]
    public async Task<IActionResult> Download(int year, int month)
    {
        List<Employee> employees = (await repository.GetAllAsync()).ToList();
        
        foreach (Employee emp in employees)
        {
            emp.Salary = await salaryService.CalculateAsync(emp.Inn);
        }

        string report = reportFormatter.Format(employees, DateUtils.FormatPeriodTitle(year, month));
        byte[] bytes = Encoding.UTF8.GetBytes(report);
        return File(bytes, "text/plain; charset=utf-8", "report.txt");

    }
}