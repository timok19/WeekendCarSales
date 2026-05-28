using System.Globalization;

namespace WeekendCarSales.Core.Extensions;

public static class DecimalExtensions
{
    private static readonly CultureInfo CzechCulture = CultureInfo.GetCultureInfo("cs-CZ");

    public static string ToCzechCurrency(this decimal amount) => amount.ToString("N2", CzechCulture);
}
