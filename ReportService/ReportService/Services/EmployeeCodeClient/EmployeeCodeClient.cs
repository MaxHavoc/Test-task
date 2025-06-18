using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace ReportService.Services.EmployeeCodeClient;

public class EmployeeCodeClient(HttpClient http, IConfiguration config)
    : IEmployeeCodeClient
{
    private readonly string _buhApiUrl = config["BuhApiUrl"];

    public async Task<string> GetCodeAsync(string inn, CancellationToken ct)
    {
        return await http.GetStringAsync(_buhApiUrl + inn, ct);
    }
}