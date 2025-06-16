using System.Globalization;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using ReportService.Domain;
using ReportService.Services.EmployeeCode;

namespace ReportService.Services.Salary;

public class SalaryService(HttpClient http, IEmployeeCodeService employeeCodeService) : ISalaryService
{
    public async Task<decimal> CalculateAsync(string inn)
    {
        string buhCode = await GetBuhCodeAsync(inn);

        string requestBody = JsonSerializer.Serialize(new { BuhCode = buhCode });
        StringContent content = new StringContent(requestBody, Encoding.UTF8, "application/json");

        HttpResponseMessage response = await http.PostAsync($"http://salary.local/api/empcode/{inn}", content);
        response.EnsureSuccessStatusCode();

        string responseText = await response.Content.ReadAsStringAsync();
        return decimal.Parse(responseText, CultureInfo.InvariantCulture);
    }

    private async Task<string> GetBuhCodeAsync(string inn)
    {
        return await employeeCodeService.GetCodeAsync(inn);
    }
}
