using System.Text;
using ReportService.Models;
using ReportService.Services.ReportFormatter;

namespace ReportService.Tests.Services;

public class ReportFormatterTests
{
    [Fact]
    public void Format_ShouldReturnCorrectReport()
    {
        var employees = new List<Employee>
        {
            new Employee { Name = "Андрей Сергеевич Бубнов", Department = "ФинОтдел", Salary = 70000 },
            new Employee { Name = "Григорий Евсеевич Зиновьев", Department = "ФинОтдел", Salary = 65000 },
            new Employee { Name = "Яков Михайлович Свердлов", Department = "ФинОтдел", Salary = 80000 },
            new Employee { Name = "Алексей Иванович Рыков", Department = "ФинОтдел", Salary = 90000 },

            new Employee { Name = "Василий Васильевич Кузнецов", Department = "Бухгалтерия", Salary = 50000 },
            new Employee { Name = "Демьян Сергеевич Коротченко", Department = "Бухгалтерия", Salary = 55000 },
            new Employee { Name = "Михаил Андреевич Суслов", Department = "Бухгалтерия", Salary = 35000 },

            new Employee { Name = "Фрол Романович Козлов", Department = "ИТ", Salary = 90000 },
            new Employee { Name = "Дмитрий Степанович Полянски", Department = "ИТ", Salary = 120000 },
            new Employee { Name = "Андрей Павлович Кириленко", Department = "ИТ", Salary = 110000 },
            new Employee { Name = "Арвид Янович Пельше", Department = "ИТ", Salary = 120000 },

            new Employee { Name = "Иван Иванович Бездеп", Department = null, Salary = 12345 }
        };

        var formatter = new ReportFormatter();
        string periodTitle = "Январь 2017";

        string expected = string.Join("\n",
            periodTitle,
            "",
            "---",
            "Без департамента",
            "",
            $"{ "Иван Иванович Бездеп",-40}{12345,10}р",
            "",
            $"{ "Всего по отделу",-40}{12345,10}р",
            "",
            "---",
            "Бухгалтерия",
            "",
            $"{ "Василий Васильевич Кузнецов",-40}{50000,10}р",
            $"{ "Демьян Сергеевич Коротченко",-40}{55000,10}р",
            $"{ "Михаил Андреевич Суслов",-40}{35000,10}р",
            "",
            $"{ "Всего по отделу",-40}{140000,10}р",
            "",
            "---",
            "ИТ",
            "",
            $"{ "Фрол Романович Козлов",-40}{90000,10}р",
            $"{ "Дмитрий Степанович Полянски",-40}{120000,10}р",
            $"{ "Андрей Павлович Кириленко",-40}{110000,10}р",
            $"{ "Арвид Янович Пельше",-40}{120000,10}р",
            "",
            $"{ "Всего по отделу",-40}{440000,10}р",
            "",
            "---",
            "ФинОтдел",
            "",
            $"{ "Андрей Сергеевич Бубнов",-40}{70000,10}р",
            $"{ "Григорий Евсеевич Зиновьев",-40}{65000,10}р",
            $"{ "Яков Михайлович Свердлов",-40}{80000,10}р",
            $"{ "Алексей Иванович Рыков",-40}{90000,10}р",
            "",
            $"{ "Всего по отделу",-40}{305000,10}р",
            "",
            "---",
            "",
            $"{ "Всего по предприятию",-40}{897345,10}р",
            ""
        ).Replace("\n", Environment.NewLine);

        string actual = formatter.Format(employees, periodTitle);

        Assert.Equal(expected, actual);
    }
    
    [Fact]
    public void Format_EmptyEmployeeList_ShouldReturnTitleAndTotalOnly()
    {
        var employees = new List<Employee>();

        var formatter = new ReportFormatter();
        string periodTitle = "Февраль 2025";

        string expected = string.Join("\n",
            periodTitle,
            "",
            "---",
            "",
            $"{ "Всего по предприятию",-40}{0,10}р",
            ""
        ).Replace("\n", System.Environment.NewLine);



        string actual = formatter.Format(employees, periodTitle);

        Assert.Equal(expected, actual);
    }
    
    [Fact]
    public void Format_ShouldHandleEmployeesWithNullOrZeroFields()
    {
        var employees = new List<Employee>
        {
            new Employee { Name = null, Department = "ИТ", Salary = 100000 },
            new Employee { Name = "Без зарплаты", Department = "ИТ", Salary = 0 },
            new Employee { Name = "Полный", Department = null, Salary = 50000 }
        };

        var formatter = new ReportFormatter();
        string periodTitle = "Март 2025";

        string expected = string.Join("\n",
            periodTitle,
            "",
            "---",
            "Без департамента",
            "",
            $"{ "Полный",-40}{50000,10}р",
            "",
            $"{ "Всего по отделу",-40}{50000,10}р",
            "",
            "---",
            "ИТ",
            "",
            $"{ "",-40}{100000,10}р",
            $"{ "Без зарплаты",-40}{0,10}р",
            "",
            $"{ "Всего по отделу",-40}{100000,10}р",
            "",
            "---",
            "",
            $"{ "Всего по предприятию",-40}{150000,10}р",
            ""
        ).Replace("\n", Environment.NewLine);

        string actual = formatter.Format(employees, periodTitle);

        Assert.Equal(expected, actual);
    }
    [Fact]
    public void Format_ShouldFormatSingleEmployee()
    {
        var employees = new List<Employee>
        {
            new Employee { Name = "Иван Иванович Иванов", Department = "ИТ", Salary = 50000 }
        };

        var formatter = new ReportFormatter();
        string periodTitle = "Март 2025";

        string expected = string.Join("\n",
            periodTitle,
            "",
            "---",
            "ИТ",
            "",
            $"{ "Иван Иванович Иванов",-40}{50000,10}р",
            "",
            $"{ "Всего по отделу",-40}{50000,10}р",
            "",
            "---",
            "",
            $"{ "Всего по предприятию",-40}{50000,10}р",
            ""
        ).Replace("\n", Environment.NewLine);

        string actual = formatter.Format(employees, periodTitle);

        Assert.Equal(expected, actual);
    }
}
