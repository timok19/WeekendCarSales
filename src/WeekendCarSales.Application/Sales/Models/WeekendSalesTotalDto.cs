namespace WeekendCarSales.Application.Sales.Models;

public sealed record WeekendSalesTotalDto(string ModelName, decimal TotalWithoutVat, decimal TotalWithVat, int WeekendSalesCount);
