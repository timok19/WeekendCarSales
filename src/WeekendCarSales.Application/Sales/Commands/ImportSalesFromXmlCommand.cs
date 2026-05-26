using FluentResults;
using WeekendCarSales.Application.Abstractions;
using WeekendCarSales.Application.Sales.Models;

namespace WeekendCarSales.Application.Sales.Commands;

public sealed class ImportSalesFromXmlCommand(ISalesXmlImporter xmlImporter, ICarSaleRepository repository)
{
    public async Task<Result<ImportSalesSummary>> Handle(string filePath, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(filePath))
        {
            return Result.Fail("Please select a valid XML file.");
        }

        var loaded = await xmlImporter.LoadAsync(filePath);
        if (loaded.IsFailed)
        {
            return Result.Fail(loaded.Errors);
        }

        await repository.ReplaceAll(loaded.Value, cancellationToken);

        return Result.Ok(new ImportSalesSummary(loaded.Value.Count, filePath));
    }
}
