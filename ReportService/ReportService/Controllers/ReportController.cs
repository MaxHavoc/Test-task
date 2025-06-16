using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Npgsql;
using ReportService.Domain;
using ReportService.Models;
using ReportService.Repositories;

namespace ReportService.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ReportController : ControllerBase
{
    private readonly IEmployeeRepository _repo;

    public ReportController(IEmployeeRepository repo)
    {
        _repo = repo;
    }
    
    [HttpGet("{year}/{month}")]
    public async Task<IActionResult> Download(int year, int month)
    {
        var report = new Report() { S = MonthNameResolver.MonthName.GetName(year, month) };
        IEnumerable<Employee> employees = await _repo.GetAllAsync();
        
        var grouped = employees
            .GroupBy(e => e.Department ?? "Без департамента")
            .Select(g => new {
                Department = g.Key,
                Employees = g.ToList()
            });
        
        foreach (Employee emp in employees)
        {
            emp.BuhCode = await EmpCodeResolver.GetCode(emp.Inn);
            emp.Salary = emp.Salary();
        }
        
        report.Save();
        var file = System.IO.File.ReadAllBytes("D:\\report.txt");
        var response = File(file, "application/octet-stream", "report.txt");
        return response;
    }
}