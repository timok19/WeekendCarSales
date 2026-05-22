using WeekendCarSales.Core.Common;

namespace WeekendCarSales.Core.Domain;

public readonly record struct VatRate
{
    public decimal Percent { get; }

    public VatRate(decimal percent)
    {
        if (percent is < 0 or > 100)
            throw new DomainException("VAT rate must be between 0 and 100 percent.");

        Percent = decimal.Round(percent, 2);
    }

    public decimal Multiplier => 1 + Percent / 100m;
}
