namespace WeekendCarSales.Core.Domain;

public sealed record CarSale
{
    public CarSale(string modelName, DateTime soldOn, Money priceWithoutVat, VatRate vatRate)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(modelName);

        ModelName = modelName.Trim();
        SoldOn = soldOn.Date;
        PriceWithoutVat = priceWithoutVat;
        VatRate = vatRate;
    }

    public CarSale(int id, string modelName, DateTime soldOn, Money priceWithoutVat, VatRate vatRate)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(modelName);

        Id = id;
        ModelName = modelName.Trim();
        SoldOn = soldOn.Date;
        PriceWithoutVat = priceWithoutVat;
        VatRate = vatRate;
    }

    public int Id { get; init; }

    public string ModelName { get; init; }

    public DateTime SoldOn { get; init; }

    public Money PriceWithoutVat { get; init; }

    public VatRate VatRate { get; init; }

    public Money PriceWithVat => new(PriceWithoutVat.Amount * VatRate.Multiplier);
}
