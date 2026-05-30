using FluentResults;
using Microsoft.Extensions.Logging;
using WeekendCarSales.Application.Abstractions;
using WeekendCarSales.Application.Sales.Models;

namespace WeekendCarSales.Application.Sales.Queries;

public sealed class GetAllCarSalesQuery(ICarSaleRepository carSaleRepository, ILogger<GetAllCarSalesQuery> logger)
{
    public async Task<Result<IReadOnlyList<CarSaleDto>>> Handle(CancellationToken cancellationToken = default)
    {
        var sales = await carSaleRepository.GetAll(cancellationToken);

        if (logger.IsEnabled(LogLevel.Debug))
            logger.LogDebug("Loaded {Count} car sales", sales.Count);

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
