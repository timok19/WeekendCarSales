using FluentResults;
using Microsoft.Extensions.Logging;
using WeekendCarSales.Application.Abstractions;
using WeekendCarSales.Application.Sales.Models;

namespace WeekendCarSales.Application.Sales.Commands;

public sealed class ImportSalesFromXmlCommand(
    ISalesXmlImporter xmlImporter,
    ICarSaleRepository repository,
    ILogger<ImportSalesFromXmlCommand> logger)
{
    public async Task<Result<ImportSalesSummary>> Handle(string filePath, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(filePath))
        {
            logger.LogWarning("Import rejected: no file path supplied.");
            return Result.Fail("Please select a valid XML file.");
        }

        logger.LogInformation("Importing car sales from {FilePath}.", filePath);

        var loaded = await xmlImporter.LoadAsync(filePath);
        if (loaded.IsFailed)
        {
            logger.LogWarning(
                "Failed to load car sales from {FilePath}: {Errors}",
                filePath,
                string.Join("; ", loaded.Errors.Select(error => error.Message)));
            return Result.Fail(loaded.Errors);
        }

        await repository.ReplaceAll(loaded.Value, cancellationToken);

        logger.LogInformation(
            "Imported {Count} car sales from {FilePath}; replaced existing records.",
            loaded.Value.Count,
            filePath);

        return Result.Ok(new ImportSalesSummary(loaded.Value.Count, filePath));
    }
}
