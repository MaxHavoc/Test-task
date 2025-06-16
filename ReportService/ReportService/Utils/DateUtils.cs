using System;
using System.Globalization;

namespace ReportService.Utils;

public static class DateUtils
{
    public static string FormatPeriodTitle(int year, int month)
    {
        if (month is < 1 or > 12)
            throw new ArgumentOutOfRangeException(nameof(month), "Месяц должен быть от 1 до 12");

        if (year < 1)
            throw new ArgumentOutOfRangeException(nameof(year), "Год должен быть положительным");

        var culture = new CultureInfo("ru-RU");
        var monthName = culture.DateTimeFormat.GetMonthName(month);
        return char.ToUpper(monthName[0]) + monthName[1..] + " " + year;
    }

}