using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ReportService.Models;

namespace ReportService.Repositories;

public interface IEmployeeRepository
{
    Task<IEnumerable<Employee>> GetAllAsync(CancellationToken ct);
}