using System.Threading;
using System.Threading.Tasks;

namespace ReportService.Services.SalaryClient;

public interface ISalaryClient
{
    Task<decimal> CalculateAsync(string inn, CancellationToken ct);
}