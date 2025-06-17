using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace ReportService.Services.EmployeeCode;

public class EmployeeCodeService(HttpClient http, IConfiguration config, ILogger<EmployeeCodeService> logger)
    : IEmployeeCodeService
{
    private readonly string _buhApiUrl = config["BuhApiUrl"];

    public async Task<string> GetCodeAsync(string inn, CancellationToken ct)
    {
        return await http.GetStringAsync(_buhApiUrl + inn, ct);
    }
}