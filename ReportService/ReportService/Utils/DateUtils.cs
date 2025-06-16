using System;
using System.Globalization;

namespace ReportService.Utils;

public static class DateUtils
{
    public static string FormatPeriodTitle(int year, int month)
    {
        var culture = new CultureInfo("ru-RU");
        var monthName = culture.DateTimeFormat.GetMonthName(month);
        return char.ToUpper(monthName[0]) + monthName[1..] + " " + year;
    }

}