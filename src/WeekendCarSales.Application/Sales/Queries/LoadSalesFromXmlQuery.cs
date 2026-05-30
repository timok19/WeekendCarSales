using FluentResults;
using Microsoft.Extensions.Logging;
using WeekendCarSales.Application.Abstractions;
using WeekendCarSales.Application.Sales.Models;

namespace WeekendCarSales.Application.Sales.Queries;

public sealed class LoadSalesFromXmlQuery(ISalesXmlImporter xmlImporter, ILogger<LoadSalesFromXmlQuery> logger)
{
    public async Task<Result<IReadOnlyList<CarSaleDto>>> Handle(string filePath, CancellationToken cancellationToken = default)
    {
        var loaded = await xmlImporter.LoadAsync(filePath);

        if (loaded.IsFailed)
        {
            logger.LogWarning(
                "Failed to load car sales from {FilePath}: {Errors}",
                filePath,
                string.Join("; ", loaded.Errors.Select(error => error.Message)));
            return Result.Fail(loaded.Errors);
        }

        var dto = loaded
            .Value.Select(
                (sale, index) =>
                    new CarSaleDto(
                        index + 1,
                        sale.ModelName,
                        sale.SoldOn,
                        sale.PriceWithoutVat.Amount,
                        sale.VatRate.Percent,
                        sale.PriceWithVat.Amount
                    )
            )
            .ToList();

        return Result.Ok<IReadOnlyList<CarSaleDto>>(dto);
    }
}
