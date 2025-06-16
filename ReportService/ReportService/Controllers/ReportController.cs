using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Npgsql;
using ReportService.Domain;
using ReportService.Models;
using ReportService.Repositories;
using ReportService.Services.Salary;

namespace ReportService.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ReportController : ControllerBase
{
    private readonly IEmployeeRepository _repository;
    private readonly ISalaryService _salaryService;

    public ReportController(IEmployeeRepository repository, ISalaryService salaryService)
    {
        _repository = repository;
        _salaryService = salaryService;
    }
    
    [HttpGet("{year}/{month}")]
    public async Task<IActionResult> Download(int year, int month)
    {
        var report = new Report() { S = "Имя месяца" };
        List<Employee> employees = (await _repository.GetAllAsync()).ToList();
        
        foreach (Employee emp in employees)
        {
            emp.Salary = await _salaryService.CalculateAsync(emp.Inn);
        }
        
        var grouped = employees
            .GroupBy(e => e.Department ?? "Без департамента")
            .Select(g => new {
                Department = g.Key,
                Employees = g.ToList()
            });
        
        
        
        report.Save();
        var file = System.IO.File.ReadAllBytes("D:\\report.txt");
        var response = File(file, "application/octet-stream", "report.txt");
        return response;
    }
}