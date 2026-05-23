using WeekendCarSales.Core.Domain;

namespace WeekendCarSales.Application.Abstractions;

public interface ICarSaleRepository
{
    Task<IReadOnlyList<CarSale>> GetAll(CancellationToken cancellationToken = default);
}
