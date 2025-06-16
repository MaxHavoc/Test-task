using System;
using System.Globalization;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using ReportService.Services.EmployeeCode;

namespace ReportService.Services.Salary;

public class SalaryService(HttpClient http, IEmployeeCodeService employeeCodeService, IConfiguration config) : ISalaryService
{
    private readonly string _salaryApiUrl = config["SalaryApiUrl"];
    public async Task<decimal> CalculateAsync(string inn, CancellationToken ct)
    {
        string buhCode = await employeeCodeService.GetCodeAsync(inn, ct);

        string requestBody = JsonSerializer.Serialize(new { BuhCode = buhCode });
        StringContent content = new StringContent(requestBody, Encoding.UTF8, "application/json");

        HttpResponseMessage response = await http.PostAsync($"{_salaryApiUrl}{inn}", content, ct);
        response.EnsureSuccessStatusCode();

        string responseText = await response.Content.ReadAsStringAsync(ct);
        if (!decimal.TryParse(responseText, NumberStyles.Number, CultureInfo.InvariantCulture, out var salary))
            throw new FormatException($"Invalid salary value returned from API: {responseText}");

        return decimal.Parse(responseText, CultureInfo.InvariantCulture);
    }
}
