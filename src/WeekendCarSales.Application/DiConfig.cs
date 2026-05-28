using Microsoft.Extensions.DependencyInjection;
using WeekendCarSales.Application.Sales.Commands;
using WeekendCarSales.Application.Sales.Queries;

namespace WeekendCarSales.Application;

public static class DiConfig
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<GetAllCarSalesQuery>();
        services.AddScoped<LoadSalesFromXmlQuery>();
        services.AddScoped<ImportSalesFromXmlCommand>();
        services.AddScoped<GetWeekendSalesReportQuery>();

        return services;
    }
}
