using System.Threading;
using System.Threading.Tasks;

namespace ReportService.Services.Salary;

public interface ISalaryService
{
    Task<decimal> CalculateAsync(string inn, CancellationToken ct);
}