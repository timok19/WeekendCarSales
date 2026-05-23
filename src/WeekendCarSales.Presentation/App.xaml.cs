using System.Windows;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using WeekendCarSales.Application;
using WeekendCarSales.Infrastructure;
using WeekendCarSales.Infrastructure.Database;
using WeekendCarSales.Presentation.ViewModels;
using WeekendCarSales.Presentation.Views;

namespace WeekendCarSales.Presentation;

public partial class App
{
    private readonly IHost _host;

    public App()
    {
        _host = Host.CreateDefaultBuilder()
            .ConfigureServices(services =>
            {
                services.AddApplication();
                services.AddInfrastructure();
                services.AddTransient<MainViewModel>();
                services.AddTransient<MainWindow>();
            })
            .Build();
    }

    protected override void OnStartup(StartupEventArgs e)
    {
        _host.StartAsync().Wait();

        using (var scope = _host.Services.CreateScope())
        {
            var dbContext = scope.ServiceProvider.GetRequiredService<WeekendCarSalesDbContext>();
            dbContext.InitializeAsync().Wait();
        }

        MainWindow = _host.Services.GetRequiredService<MainWindow>();
        MainWindow.Show();

        base.OnStartup(e);
    }

    protected override void OnExit(ExitEventArgs e)
    {
        _host.StopAsync().Wait();
        _host.Dispose();

        base.OnExit(e);
    }
}
