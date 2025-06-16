using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
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
public class ReportController(
    IEmployeeRepository repository,
    ISalaryService salaryService,
    IReportFormatter reportFormatter)
    : ControllerBase
{
    [HttpGet("{year}/{month}")]
    public async Task<IActionResult> Download(int year, int month, CancellationToken ct)
    {
        if (month < 1 || month > 12)
            return BadRequest("Месяц должен быть от 1 до 12");

        if (year < 1)
            return BadRequest("Год должен быть положительным");

        var employees = (await repository.GetAllAsync(ct)).ToList();

        var salaryTasks = employees.Select(async emp =>
        {
            emp.Salary = await salaryService.CalculateAsync(emp.Inn, ct);
            return emp;
        });

        await Task.WhenAll(salaryTasks);

        string report = reportFormatter.Format(employees, DateUtils.FormatPeriodTitle(year, month));
        var bytes = Encoding.UTF8.GetBytes(report);

        return File(bytes, "text/plain; charset=utf-8", "report.txt");
    }
}
