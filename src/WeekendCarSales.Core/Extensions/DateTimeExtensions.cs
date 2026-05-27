namespace WeekendCarSales.Core.Extensions;

public static class DateTimeExtensions
{
    public static bool IsWeekend(this DateTime date) => date.DayOfWeek is DayOfWeek.Saturday or DayOfWeek.Sunday;
}
