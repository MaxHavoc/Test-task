using System.Collections.Generic;
using System.Linq;
using System.Text;
using ReportService.Models;

namespace ReportService.Services.ReportFormatter;

public class ReportFormatter : IReportFormatter
{
    public string Format(IEnumerable<Employee> employees, string periodTitle)
    {
        StringBuilder sb = new ();
        sb.AppendLine(periodTitle);
        sb.AppendLine();

        IOrderedEnumerable<IGrouping<string, Employee>> grouped = employees
            .GroupBy(e => e.Department ?? "Без департамента")
            .OrderBy(g => g.Key);

        decimal total = 0;

        foreach (IGrouping<string, Employee> group in grouped)
        {
            if (!group.Any())
                continue;
            
            sb.AppendLine("---");
            sb.AppendLine(group.Key);
            sb.AppendLine();

            decimal depTotal = 0;

            foreach (Employee emp in group)
            {
                sb.AppendLine($"{emp.Name,-40}{emp.Salary,10:0}р");

                depTotal += emp.Salary;
            }

            total += depTotal;

            sb.AppendLine();
            sb.AppendLine($"{ "Всего по отделу",-40}{depTotal,10:0}р");
            sb.AppendLine();
        }

        sb.AppendLine("---");
        sb.AppendLine();
        sb.AppendLine($"{ "Всего по предприятию",-40}{total,10:0}р");

        return sb.ToString();
    }
}
