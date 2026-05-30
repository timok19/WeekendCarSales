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
        await dbContext.CarSales.AddRangeAsync(ToDatabase(sales), cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<CarSale>> GetAll(CancellationToken cancellationToken = default)
    {
        var carSales = await dbContext
            .CarSales.AsNoTracking()
            .OrderBy(sale => sale.SoldOn)
            .ThenBy(sale => sale.ModelName)
            .ToListAsync(cancellationToken);

        return ToDomain(carSales);
    }

    public async Task<IReadOnlyList<CarSale>> GetWeekendSales(CancellationToken cancellationToken = default)
    {
        var carSales = await dbContext
            .CarSales.AsNoTracking()
            .Where(sale => sale.SoldOn.DayOfWeek == DayOfWeek.Saturday || sale.SoldOn.DayOfWeek == DayOfWeek.Sunday)
            .ToListAsync(cancellationToken);

        return ToDomain(carSales);
    }

    private static List<CarSale> ToDomain(IEnumerable<Database.Models.CarSale> carSales) =>
        carSales.Select(ToDomain).ToList();

    private static CarSale ToDomain(Database.Models.CarSale carSale) =>
        new(
            carSale.ModelName,
            carSale.SoldOn,
            new Money(carSale.PriceWithoutVat.Amount),
            new VatRate(carSale.VatRate.Percent)
        );

    private static List<Database.Models.CarSale> ToDatabase(IEnumerable<CarSale> carSales)
        => carSales.Select(ToDatabase).ToList();

    private static Database.Models.CarSale ToDatabase(CarSale carSale)
        => new(
            carSale.ModelName,
            carSale.SoldOn,
            new Database.Models.Money(carSale.PriceWithoutVat.Amount),
            new Database.Models.VatRate(carSale.VatRate.Percent)
        );
}
