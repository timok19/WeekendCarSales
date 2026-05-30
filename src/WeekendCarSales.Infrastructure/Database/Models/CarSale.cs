using System.ComponentModel.DataAnnotations;

namespace WeekendCarSales.Infrastructure.Database.Models;

public class CarSale
{
    public CarSale(string modelName, DateTime soldOn, Money priceWithoutVat, VatRate vatRate)
    {
        ModelName = modelName;
        SoldOn = soldOn;
        PriceWithoutVat = priceWithoutVat;
        VatRate = vatRate;
    }

    public CarSale(int id, string modelName, DateTime soldOn, Money priceWithoutVat, VatRate vatRate)
    {
        Id = id;
        ModelName = modelName;
        SoldOn = soldOn;
        PriceWithoutVat = priceWithoutVat;
        VatRate = vatRate;
    }

    public int Id { get; set; }

    [MaxLength(255)]
    public string ModelName { get; set; }

    public DateTime SoldOn { get; set; }

    public Money PriceWithoutVat { get; set; }

    public VatRate VatRate { get; set; }
}
