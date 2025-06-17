using System.Text;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using ReportService.Controllers;
using ReportService.Models;
using ReportService.Repositories;
using ReportService.Services.ReportFormatter;
using ReportService.Services.Salary;

namespace ReportService.Tests.Controllers;

public class ReportControllerTests
{
    private readonly Mock<IEmployeeRepository> _repoMock = new();
    private readonly Mock<ISalaryClient> _salaryMock = new();
    private readonly Mock<IReportFormatter> _formatterMock = new();
    private readonly Mock<ILogger<ReportController>> _loggerMock = new();

    [Fact]
    public async Task Download_ReturnsBadRequest_WhenMonthInvalid()
    {
        var controller = new ReportController(
            _repoMock.Object,
            _salaryMock.Object,
            _formatterMock.Object,
            _loggerMock.Object);

        var result = await controller.Download(2025, 0, CancellationToken.None);

        Assert.IsType<BadRequestObjectResult>(result);
    }

    [Fact]
    public async Task Download_ReturnsFileResult_WithCorrectReport()
    {
        var employees = new List<Employee> {
            new Employee { Inn = "123", Salary = 0 }
        };
        _repoMock.Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>())).ReturnsAsync(employees);

        _salaryMock.Setup(s => s.CalculateAsync("123", It.IsAny<CancellationToken>())).ReturnsAsync(1000m);

        _formatterMock.Setup(f => f.Format(It.IsAny<IEnumerable<Employee>>(), It.IsAny<string>()))
            .Returns("ReportContent");

        var controller = new ReportController(
            _repoMock.Object,
            _salaryMock.Object,
            _formatterMock.Object,
            _loggerMock.Object);

        var result = await controller.Download(2025, 5, CancellationToken.None);

        var fileResult = Assert.IsType<FileContentResult>(result);
        Assert.Equal("text/plain; charset=utf-8", fileResult.ContentType);
        Assert.Equal("report.txt", fileResult.FileDownloadName);
        Assert.Equal(Encoding.UTF8.GetBytes("ReportContent"), fileResult.FileContents);

        _repoMock.Verify(r => r.GetAllAsync(It.IsAny<CancellationToken>()), Times.Once);
        _salaryMock.Verify(s => s.CalculateAsync("123", It.IsAny<CancellationToken>()), Times.Once);
        _formatterMock.Verify(f => f.Format(It.IsAny<IEnumerable<Employee>>(), "Май 2025"), Times.Once);
    }
    
    [Fact]
    public async Task Download_ShouldCalculateSalaryAndReturnReportFile()
    {
        var employees = new List<Employee>
        {
            new() { Name = "Иван", Inn = "123", Salary = 0 },
            new() { Name = "Петр", Inn = "456", Salary = 0 }
        };

        var repositoryMock = new Mock<IEmployeeRepository>();
        repositoryMock.Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(employees);

        var salaryClientMock = new Mock<ISalaryClient>();
        salaryClientMock.Setup(c => c.CalculateAsync("123", It.IsAny<CancellationToken>()))
            .ReturnsAsync(1000);
        salaryClientMock.Setup(c => c.CalculateAsync("456", It.IsAny<CancellationToken>()))
            .ReturnsAsync(2000);

        var formatterMock = new Mock<IReportFormatter>();
        formatterMock.Setup(f => f.Format(It.IsAny<List<Employee>>(), It.IsAny<string>()))
            .Returns("тестовый отчет");

        var loggerMock = new Mock<ILogger<ReportController>>();

        var controller = new ReportController(
            repositoryMock.Object,
            salaryClientMock.Object,
            formatterMock.Object,
            loggerMock.Object
        );
        
        var result = await controller.Download(2025, 2, CancellationToken.None);
        
        Assert.IsType<FileContentResult>(result);
        var fileResult = (FileContentResult)result;
        var content = Encoding.UTF8.GetString(fileResult.FileContents);
        Assert.Equal("тестовый отчет", content);

        Assert.Equal(1000, employees[0].Salary);
        Assert.Equal(2000, employees[1].Salary);

        salaryClientMock.Verify(c => c.CalculateAsync("123", It.IsAny<CancellationToken>()), Times.Once);
        salaryClientMock.Verify(c => c.CalculateAsync("456", It.IsAny<CancellationToken>()), Times.Once);
    }

}
