using System.Net.Http;
using System.Threading.Tasks;

namespace ReportService.Services.EmployeeCode;

public class EmployeeCodeService(HttpClient http) : IEmployeeCodeService
{
    public async Task<string> GetCodeAsync(string inn)
    {
        return await http.GetStringAsync($"http://buh.local/api/inn/{inn}");
    }
}