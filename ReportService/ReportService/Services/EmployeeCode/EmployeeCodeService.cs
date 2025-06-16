using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;

namespace ReportService.Services.EmployeeCode;

public class EmployeeCodeService(HttpClient http, IConfiguration config) : IEmployeeCodeService
{
    private readonly string _buhApiUrl = config["BuhApiUrl"];

    public async Task<string> GetCodeAsync(string inn, CancellationToken ct)
    {
        return await http.GetStringAsync(_buhApiUrl + inn, ct);
    }
}