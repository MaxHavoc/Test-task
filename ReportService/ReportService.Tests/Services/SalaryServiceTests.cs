using System.Net;
using Microsoft.Extensions.Configuration;
using Moq;
using Moq.Protected;
using ReportService.Services.EmployeeCode;
using ReportService.Services.Salary;

namespace ReportService.Tests.Services;

public class SalaryServiceTests
{
    private const string Inn = "1234567890";
    private const string BuhCode = "ABC123";
    private const string SalaryApiUrl = "http://salary.local/api/empcode/";

    private readonly Mock<IEmployeeCodeService> _employeeCodeServiceMock;
    private readonly Mock<HttpMessageHandler> _handlerMock;
    private readonly SalaryService _service;

    public SalaryServiceTests()
    {
        _employeeCodeServiceMock = new Mock<IEmployeeCodeService>();
        _employeeCodeServiceMock.Setup(s => s.GetCodeAsync(Inn, It.IsAny<CancellationToken>()))
            .ReturnsAsync(BuhCode);

        _handlerMock = new Mock<HttpMessageHandler>();

        var httpClient = new HttpClient(_handlerMock.Object);

        var configurationMock = new Mock<IConfiguration>();
        configurationMock.Setup(c => c["SalaryApiUrl"]).Returns(SalaryApiUrl);

        _service = new SalaryService(httpClient, _employeeCodeServiceMock.Object, configurationMock.Object);
    }

    [Fact]
    public async Task CalculateAsync_ReturnsSalary_WhenResponseIsValid()
    {
        _handlerMock.Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.Is<HttpRequestMessage>(req =>
                    req.Method == HttpMethod.Post &&
                    req.RequestUri == new Uri(SalaryApiUrl + Inn)),
                ItExpr.IsAny<CancellationToken>()
            )
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent("75000.50"),
            });

        decimal salary = await _service.CalculateAsync(Inn, CancellationToken.None);

        Assert.Equal(75000.50m, salary);
        _employeeCodeServiceMock.Verify(s => s.GetCodeAsync(Inn, It.IsAny<CancellationToken>()), Times.Once);
        _handlerMock.Protected().Verify(
            "SendAsync",
            Times.Once(),
            ItExpr.Is<HttpRequestMessage>(req => req.Method == HttpMethod.Post),
            ItExpr.IsAny<CancellationToken>());
    }

    [Fact]
    public async Task CalculateAsync_ThrowsFormatException_WhenResponseIsInvalid()
    {
        _handlerMock.Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent("not-a-number"),
            });

        await Assert.ThrowsAsync<FormatException>(() => _service.CalculateAsync(Inn, CancellationToken.None));
    }
}
