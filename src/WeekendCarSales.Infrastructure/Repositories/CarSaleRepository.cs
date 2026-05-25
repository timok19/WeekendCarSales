using Microsoft.EntityFrameworkCore;
using WeekendCarSales.Application.Abstractions;
using WeekendCarSales.Core.Domain;
using WeekendCarSales.Infrastructure.Database;

namespace WeekendCarSales.Infrastructure.Repositories;

public sealed class CarSaleRepository(WeekendCarSalesDbContext dbContext) : ICarSaleRepository
{
    public async Task ReplaceAll(IEnumerable<CarSale> sales, CancellationToken cancellationToken = default)
    {
        await dbContext.CarSales.ExecuteDeleteAsync(cancellationToken);
        await dbContext.CarSales.AddRangeAsync(sales, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<CarSale>> GetAll(CancellationToken cancellationToken = default)
    {
        return await dbContext
            .CarSales.AsNoTracking()
            .OrderBy(sale => sale.SoldOn)
            .ThenBy(sale => sale.ModelName)
            .ToListAsync(cancellationToken);
    }
}
