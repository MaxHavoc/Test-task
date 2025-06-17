using System.Threading;
using System.Threading.Tasks;

namespace ReportService.Services.EmployeeCode;

public interface IEmployeeCodeClient
{
    Task<string> GetCodeAsync(string inn, CancellationToken ct);
}