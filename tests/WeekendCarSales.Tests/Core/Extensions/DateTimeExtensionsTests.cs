using WeekendCarSales.Core.Extensions;
using Shouldly;
using Xunit;

namespace WeekendCarSales.Tests.Core.Extensions;

public sealed class DateTimeExtensionsTests
{
    [Theory]
    [InlineData(2010, 12, 4, true)]
    [InlineData(2010, 12, 5, true)]
    [InlineData(2010, 12, 6, false)]
    public void IsWeekend_ReturnsExpectedResult(int year, int month, int day, bool expected)
    {
        var actual = new DateTime(year, month, day).IsWeekend();

        actual.ShouldBe(expected);
    }
}
