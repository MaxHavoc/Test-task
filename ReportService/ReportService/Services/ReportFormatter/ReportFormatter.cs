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
            sb.AppendLine("---");
            sb.AppendLine(group.Key);
            sb.AppendLine();

            decimal depTotal = 0;

            foreach (Employee emp in group)
            {
                string namePadded = emp.Name.PadRight(40);
                string salaryFormatted = $"{emp.Salary:0}р";
                sb.AppendLine($"{namePadded}{salaryFormatted}");

                depTotal += emp.Salary;
            }

            total = depTotal;

            sb.AppendLine();
            sb.AppendLine($"Всего по отделу\t\t\t{depTotal:0}р");
            sb.AppendLine();
        }

        sb.AppendLine("---");
        sb.AppendLine();
        sb.AppendLine($"Всего по предприятию\t\t\t{total:0}р");

        return sb.ToString();
    }
}
