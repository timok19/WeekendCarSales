using Microsoft.EntityFrameworkCore;
using WeekendCarSales.Infrastructure.Database.Models;

namespace WeekendCarSales.Infrastructure.Database;

public sealed class WeekendCarSalesDbContext(DbContextOptions<WeekendCarSalesDbContext> options) : DbContext(options)
{
    private const string DatabaseFileName = "weekend-car-sales.db";

    public DbSet<CarSale> CarSales => Set<CarSale>();

    public static string GetDatabasePath()
    {
        var dataDirectory = Path.Combine(AppContext.BaseDirectory, "Data");

        Directory.CreateDirectory(dataDirectory);

        return Path.Combine(dataDirectory, DatabaseFileName);
    }

    public async Task InitializeAsync(CancellationToken cancellationToken = default)
    {
        await Database.MigrateAsync(cancellationToken);

        if (await CarSales.AnyAsync(cancellationToken))
            return;

        var seedData = new CarSale[]
        {
            new("Škoda Oktávia", new DateTime(2010, 12, 02), new Money(500000.00m), new VatRate(20.00m)),
            new("Škoda Felicia", new DateTime(2000, 12, 03), new Money(210000.00m), new VatRate(20.00m)),
            new("Škoda Fabia", new DateTime(2010, 12, 04), new Money(350000.00m), new VatRate(20.00m)),
            new("Škoda Oktávia", new DateTime(2010, 12, 04), new Money(500000.00m), new VatRate(20.00m)),
            new("Škoda Oktávia", new DateTime(2010, 12, 05), new Money(500000.00m), new VatRate(20.00m)),
            new("Škoda Fabia", new DateTime(2010, 12, 05), new Money(350000.00m), new VatRate(20.00m)),
            new("Škoda Fabia", new DateTime(2010, 12, 06), new Money(350000.00m), new VatRate(20.00m)),
            new("Škoda Forman", new DateTime(2000, 12, 04), new Money(100000.00m), new VatRate(19.00m)),
            new("Škoda Favorit", new DateTime(2000, 12, 05), new Money(80000.00m), new VatRate(19.00m)),
            new("Škoda Forman", new DateTime(2000, 12, 06), new Money(100000.00m), new VatRate(19.00m)),
            new("Škoda Felicia", new DateTime(2000, 12, 03), new Money(210000.00m), new VatRate(19.00m)),
            new("Škoda Felicia", new DateTime(2000, 12, 02), new Money(210000.00m), new VatRate(19.00m)),
            new("Škoda Oktávia", new DateTime(2010, 12, 07), new Money(500000.00m), new VatRate(20.00m)),
        };

        await CarSales.AddRangeAsync(seedData, cancellationToken);

        await SaveChangesAsync(cancellationToken);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<CarSale>(builder =>
        {
            builder.ToTable("CarSales");

            builder.HasKey(sale => sale.Id);

            builder.Property(sale => sale.Id).ValueGeneratedOnAdd();

            builder.Property(sale => sale.ModelName).HasMaxLength(120).IsRequired();

            builder.Property(sale => sale.SoldOn).HasColumnType("TEXT").IsRequired();

            builder
                .Property(sale => sale.PriceWithoutVat)
                .HasConversion(money => money.Amount, amount => new Money(amount))
                .HasColumnName("PriceWithoutVat")
                .HasColumnType("decimal(18,2)")
                .IsRequired();

            builder
                .Property(sale => sale.VatRate)
                .HasConversion(vatRate => vatRate.Percent, percent => new VatRate(percent))
                .HasColumnName("VatRate")
                .HasColumnType("decimal(5,2)")
                .IsRequired();
        });
    }
}
