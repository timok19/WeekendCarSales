using FluentResults;
using WeekendCarSales.Application.Abstractions;
using WeekendCarSales.Application.Sales.Models;

namespace WeekendCarSales.Application.Sales.Queries;

public sealed class GetAllCarSalesQuery(ICarSaleRepository carSaleRepository)
{
    public async Task<Result<IReadOnlyList<CarSaleDto>>> Handle(CancellationToken cancellationToken = default)
    {
        var sales = await carSaleRepository.GetAll(cancellationToken);

        var dto = sales
            .OrderBy(sale => sale.SoldOn)
            .ThenBy(sale => sale.ModelName)
            .Select(sale => new CarSaleDto(
                sale.Id,
                sale.ModelName,
                sale.SoldOn,
                sale.PriceWithoutVat.Amount,
                sale.VatRate.Percent,
                sale.PriceWithVat.Amount
            ))
            .ToList();

        return Result.Ok<IReadOnlyList<CarSaleDto>>(dto);
    }
}
