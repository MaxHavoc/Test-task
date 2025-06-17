using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using ReportService.Models;
using ReportService.Repositories;
using ReportService.Services.ReportFormatter;
using ReportService.Services.SalaryClient;
using ReportService.Utils;

namespace ReportService.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ReportController(
    IEmployeeRepository repository,
    ISalaryClient salaryService,
    IReportFormatter reportFormatter,
    ILogger<ReportController> logger)
    : ControllerBase
{
    [HttpGet("{year}/{month}")]
    public async Task<IActionResult> Download(int year, int month, CancellationToken ct)
    {
        logger.LogInformation($"Report generation requested for {year}/{month}");
        
        if (month < 1 || month > 12)
            return BadRequest("Month must be between 1 and 12");

        if (year < 1)
            return BadRequest("Year must be a positive number");

        List<Employee> employees = (await repository.GetAllAsync(ct)).ToList();
        
        logger.LogInformation($"Retrieved {employees.Count} employees from database");
        
        List<Task> tasks = [];
        foreach (Employee emp in employees)
        {
            tasks.Add(Task.Run(async () =>
            {
                emp.Salary = await salaryService.CalculateAsync(emp.Inn, ct);
            }, ct));
        }
        await Task.WhenAll(tasks);

        string report = reportFormatter.Format(employees, DateUtils.FormatPeriodTitle(year, month));
        var bytes = Encoding.UTF8.GetBytes(report);
        
        logger.LogInformation($"Report generated, size {bytes.Length} bytes");
        return File(bytes, "text/plain; charset=utf-8", "report.txt");
    }
}
