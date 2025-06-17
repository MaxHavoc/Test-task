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
}
