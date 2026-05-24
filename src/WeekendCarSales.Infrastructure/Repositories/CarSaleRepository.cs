using Microsoft.EntityFrameworkCore;
using WeekendCarSales.Application.Abstractions;
using WeekendCarSales.Core.Domain;
using WeekendCarSales.Infrastructure.Database;

namespace WeekendCarSales.Infrastructure.Repositories;

public sealed class CarSaleRepository(WeekendCarSalesDbContext dbContext) : ICarSaleRepository
{
    public async Task<IReadOnlyList<CarSale>> GetAll(CancellationToken cancellationToken = default)
    {
        return await dbContext
            .CarSales.AsNoTracking()
            .OrderBy(sale => sale.SoldOn)
            .ThenBy(sale => sale.ModelName)
            .ToListAsync(cancellationToken);
    }
}
