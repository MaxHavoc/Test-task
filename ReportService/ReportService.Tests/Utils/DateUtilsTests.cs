using ReportService.Utils;

namespace ReportService.Tests.Utils;

public class DateUtilsTests
{
    [Fact]
    public void FormatPeriodTitle_ValidMonth_ReturnsCorrectTitle()
    {
        string result = DateUtils.FormatPeriodTitle(2025, 2);
        Assert.Equal("Февраль 2025", result);
    }

    [Fact]
    public void FormatPeriodTitle_AnotherValidMonth_ReturnsCorrectTitle()
    {
        string result = DateUtils.FormatPeriodTitle(1990, 12);
        Assert.Equal("Декабрь 1990", result);
    }

    [Theory]
    [InlineData(2025, 0)]
    [InlineData(2025, 13)]
    public void FormatPeriodTitle_InvalidMonth_ThrowsArgumentOutOfRangeException(int year, int month)
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => DateUtils.FormatPeriodTitle(year, month));
    }
}