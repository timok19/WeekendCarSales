using WeekendCarSales.Core.Domain;

namespace WeekendCarSales.Application.Abstractions;

public interface ICarSaleRepository
{
    Task ReplaceAll(IEnumerable<CarSale> sales, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<CarSale>> GetAll(CancellationToken cancellationToken = default);

    Task<IReadOnlyList<CarSale>> GetWeekendSales(CancellationToken cancellationToken = default);
}
