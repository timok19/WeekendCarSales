namespace WeekendCarSales.Application.Sales.Models;

public sealed record CarSaleDto(
    int Id,
    string ModelName,
    DateTime SoldOn,
    decimal PriceWithoutVat,
    decimal VatPercent,
    decimal PriceWithVat
);
