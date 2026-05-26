using FluentResults;
using WeekendCarSales.Core.Domain;

namespace WeekendCarSales.Application.Abstractions;

public interface ISalesXmlImporter
{
    Task<Result<IReadOnlyList<CarSale>>> LoadAsync(string filePath);
}
