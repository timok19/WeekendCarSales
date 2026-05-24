using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;
using WeekendCarSales.Application.Sales.Models;
using WeekendCarSales.Application.Sales.Queries;

namespace WeekendCarSales.Presentation.ViewModels;

public sealed class MainViewModel : ViewModelBase
{
    private readonly GetAllCarSalesQuery _getAllCarSalesQuery;

    public MainViewModel(GetAllCarSalesQuery getAllCarSalesQuery)
    {
        _getAllCarSalesQuery = getAllCarSalesQuery;

        RefreshCommand = new AsyncRelayCommand(RefreshAsync);

        _ = RefreshAsync();
    }

    public ObservableCollection<CarSaleDto> Sales { get; } = [];

    public ICommand RefreshCommand { get; }

    private async Task RefreshAsync()
    {
        var allSales = await _getAllCarSalesQuery.Handle();

        if (allSales.IsFailed)
        {
            MessageBox.Show(
                string.Join(Environment.NewLine, allSales.Errors.Select(error => error.Message)),
                "Weekend Car Sales",
                MessageBoxButton.OK,
                MessageBoxImage.Warning
            );
            return;
        }

        Sales.Clear();
        foreach (var sale in allSales.Value)
            Sales.Add(sale);
    }
}
