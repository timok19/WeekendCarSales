using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Shouldly;
using WeekendCarSales.Core.Domain;
using WeekendCarSales.Infrastructure.Database;
using WeekendCarSales.Infrastructure.Repositories;
using Xunit;

namespace WeekendCarSales.Tests.Infrastructure.Repositories;

public sealed class CarSaleRepositoryTests : IDisposable
{
    private readonly SqliteConnection _connection;
    private readonly WeekendCarSalesDbContext _dbContext;
    private readonly CarSaleRepository _repository;

    public CarSaleRepositoryTests()
    {
        // A shared in-memory database lives only as long as its connection is open.
        _connection = new SqliteConnection("DataSource=:memory:");
        _connection.Open();

        var options = new DbContextOptionsBuilder<WeekendCarSalesDbContext>()
            .UseSqlite(_connection)
            .Options;

        _dbContext = new WeekendCarSalesDbContext(options);
        _dbContext.Database.EnsureCreated();

        _repository = new CarSaleRepository(_dbContext);
    }

    [Fact]
    public async Task GetWeekendSales_ReturnsOnlySaturdayAndSundaySales()
    {
        await _repository.ReplaceAll(SampleSales());

        var weekendSales = await _repository.GetWeekendSales();

        weekendSales.ShouldAllBe(sale => sale.SoldOn.DayOfWeek == DayOfWeek.Saturday || sale.SoldOn.DayOfWeek == DayOfWeek.Sunday);
        weekendSales.Count.ShouldBe(7);

        // Felicia has mixed VAT rates on the weekend (20% + 19% + 19%) — all three must be present.
        weekendSales.Count(sale => sale.ModelName == "Škoda Felicia").ShouldBe(3);
        weekendSales.Count(sale => sale.ModelName == "Škoda Fabia").ShouldBe(2);
        weekendSales.Count(sale => sale.ModelName == "Škoda Oktávia").ShouldBe(2);

        // A weekday-only model must be excluded entirely.
        weekendSales.ShouldNotContain(sale => sale.ModelName == "Škoda Favorit");
        weekendSales.ShouldNotContain(sale => sale.ModelName == "Škoda Forman");
    }

    [Fact]
    public async Task ReplaceAll_PersistsValueObjectsAndIsRoundTrippable()
    {
        var carSale = new CarSale("Škoda Fabia", new DateTime(2010, 12, 4), new Money(350000.00m), new VatRate(20.00m));

        await _repository.ReplaceAll([carSale]);

        var all = await _repository.GetAll();

        var sale = all.ShouldHaveSingleItem();
        sale.ModelName.ShouldBe("Škoda Fabia");
        sale.PriceWithoutVat.Amount.ShouldBe(350000.00m);
        sale.VatRate.Percent.ShouldBe(20.00m);
        sale.PriceWithVat.Amount.ShouldBe(420000.00m);
    }

    [Fact]
    public async Task ReplaceAll_OverwritesPreviousData()
    {
        var carSaleOld = new CarSale("Old Model", new DateTime(2010, 12, 4), new Money(100m), new VatRate(20m));
        var carSaleNew = new CarSale("New Model", new DateTime(2010, 12, 5), new Money(200m), new VatRate(20m));

        await _repository.ReplaceAll([carSaleOld]);
        await _repository.ReplaceAll([carSaleNew]);

        var all = await _repository.GetAll();

        all.ShouldHaveSingleItem().ModelName.ShouldBe("New Model");
    }

    private static IReadOnlyList<CarSale> SampleSales() =>
    [
        new("Škoda Oktávia", new DateTime(2010, 12, 2), new Money(500000m), new VatRate(20m)), // Thu
        new("Škoda Felicia", new DateTime(2000, 12, 3), new Money(210000m), new VatRate(20m)), // Sun
        new("Škoda Fabia", new DateTime(2010, 12, 4), new Money(350000m), new VatRate(20m)),   // Sat
        new("Škoda Oktávia", new DateTime(2010, 12, 4), new Money(500000m), new VatRate(20m)), // Sat
        new("Škoda Oktávia", new DateTime(2010, 12, 5), new Money(500000m), new VatRate(20m)), // Sun
        new("Škoda Fabia", new DateTime(2010, 12, 5), new Money(350000m), new VatRate(20m)),   // Sun
        new("Škoda Fabia", new DateTime(2010, 12, 6), new Money(350000m), new VatRate(20m)),   // Mon
        new("Škoda Forman", new DateTime(2000, 12, 4), new Money(100000m), new VatRate(19m)),  // Mon
        new("Škoda Favorit", new DateTime(2000, 12, 5), new Money(80000m), new VatRate(19m)),  // Tue
        new("Škoda Forman", new DateTime(2000, 12, 6), new Money(100000m), new VatRate(19m)),  // Wed
        new("Škoda Felicia", new DateTime(2000, 12, 3), new Money(210000m), new VatRate(19m)), // Sun
        new("Škoda Felicia", new DateTime(2000, 12, 2), new Money(210000m), new VatRate(19m)), // Sat
        new("Škoda Oktávia", new DateTime(2010, 12, 7), new Money(500000m), new VatRate(20m)), // Tue
    ];

    public void Dispose()
    {
        _dbContext.Dispose();
        _connection.Dispose();
    }
}
