using System;
using System.Globalization;

namespace ReportService.Utils;

public static class DateUtils
{
    public static string FormatPeriodTitle(int year, int month)
    {
        if (month < 1 || month > 12)
            throw new ArgumentOutOfRangeException(nameof(month), "Month must be between 1 and 12.");
        
        var culture = new CultureInfo("ru-RU");
        var monthName = culture.DateTimeFormat.GetMonthName(month);
        return char.ToUpper(monthName[0]) + monthName[1..] + " " + year;
    }

}