using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using WeekendCarSales.Application.Abstractions;
using WeekendCarSales.Infrastructure.Database;
using WeekendCarSales.Infrastructure.Repositories;
using WeekendCarSales.Infrastructure.Services;

namespace WeekendCarSales.Infrastructure;

public static class DiConfig
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services)
    {
        var dbPath = WeekendCarSalesDbContext.GetDatabasePath();

        services.AddDbContext<WeekendCarSalesDbContext>(options => options.UseSqlite($"Data Source={dbPath}"));

        services.AddScoped<ICarSaleRepository, CarSaleRepository>();
        services.AddScoped<ISalesXmlImporter, SalesXmlImporter>();

        return services;
    }
}
