using System.Collections.Generic;
using ReportService.Models;

namespace ReportService.Services.ReportFormatter;

public interface IReportFormatter
{
    string Format(IEnumerable<Employee> employees, string periodTitle);
}