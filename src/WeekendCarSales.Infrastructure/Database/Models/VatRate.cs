namespace WeekendCarSales.Infrastructure.Database.Models;

public class VatRate
{
    public VatRate(decimal percent)
    {
        Percent = percent;
    }

    public decimal Percent { get; set; }
}
