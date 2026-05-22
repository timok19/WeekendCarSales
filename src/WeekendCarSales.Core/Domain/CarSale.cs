namespace WeekendCarSales.Core.Domain;

public sealed class CarSale
{
    public CarSale(string modelName, DateTime soldOn, Money priceWithoutVat, VatRate vatRate)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(modelName);

        ModelName = modelName.Trim();
        SoldOn = soldOn.Date;
        PriceWithoutVat = priceWithoutVat;
        VatRate = vatRate;
    }

    public int Id { get; private set; }

    public string ModelName { get; private set; }

    public DateTime SoldOn { get; private set; }

    public Money PriceWithoutVat { get; private set; }

    public VatRate VatRate { get; private set; }

    public Money PriceWithVat => new(PriceWithoutVat.Amount * VatRate.Multiplier);
}
