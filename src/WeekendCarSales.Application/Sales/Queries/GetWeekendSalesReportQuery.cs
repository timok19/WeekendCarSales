using FluentResults;
using WeekendCarSales.Application.Abstractions;
using WeekendCarSales.Application.Sales.Models;

namespace WeekendCarSales.Application.Sales.Queries;

public sealed class GetWeekendSalesReportQuery(ICarSaleRepository repository)
{
    public async Task<Result<IReadOnlyList<WeekendSalesTotalDto>>> Handle(CancellationToken cancellationToken = default)
    {
        var weekendSales = await repository.GetWeekendSales(cancellationToken);

        var dto = weekendSales
            .GroupBy(sale => sale.ModelName)
            .OrderBy(group => group.Key)
            .Select(group => new WeekendSalesTotalDto(
                group.Key,
                group.Sum(sale => sale.PriceWithoutVat.Amount),
                group.Sum(sale => sale.PriceWithVat.Amount),
                group.Count()
            ))
            .ToList();

        return Result.Ok<IReadOnlyList<WeekendSalesTotalDto>>(dto);
    }
}
