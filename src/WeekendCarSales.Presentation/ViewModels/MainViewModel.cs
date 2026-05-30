using System.Collections.ObjectModel;
using System.IO;
using System.Windows;
using System.Windows.Input;
using FluentResults;
using Microsoft.Win32;
using WeekendCarSales.Application.Sales.Commands;
using WeekendCarSales.Application.Sales.Models;
using WeekendCarSales.Application.Sales.Queries;
using WeekendCarSales.Core.Extensions;
using WeekendCarSales.Presentation.Extensions;

namespace WeekendCarSales.Presentation.ViewModels;

public sealed class MainViewModel : ViewModelBase
{
    private readonly ImportSalesFromXmlCommand _importSalesFromXmlCommand;
    private readonly LoadSalesFromXmlQuery _loadSalesFromXmlQuery;
    private readonly GetAllCarSalesQuery _getAllCarSalesQuery;
    private readonly GetWeekendSalesReportQuery _getWeekendSalesReportQuery;

    public MainViewModel(
        ImportSalesFromXmlCommand importSalesFromXmlCommand,
        LoadSalesFromXmlQuery loadSalesFromXmlQuery,
        GetAllCarSalesQuery getAllCarSalesQuery,
        GetWeekendSalesReportQuery getWeekendSalesReportQuery
    )
    {
        _importSalesFromXmlCommand = importSalesFromXmlCommand;
        _loadSalesFromXmlQuery = loadSalesFromXmlQuery;
        _getAllCarSalesQuery = getAllCarSalesQuery;
        _getWeekendSalesReportQuery = getWeekendSalesReportQuery;

        BrowseCommand = new AsyncRelayCommand(BrowseAsync);
        ImportCommand = new AsyncRelayCommand(ImportAsync, () => !string.IsNullOrWhiteSpace(SelectedFilePath));
        LoadSampleCommand = new AsyncRelayCommand(LoadSampleAsync);
        RefreshReportCommand = new AsyncRelayCommand(RefreshAsync);
    }

    public ObservableCollection<CarSaleDto> Sales { get; } = [];

    public ObservableCollection<WeekendSalesTotalDto> WeekendTotals { get; } = [];

    public ICommand BrowseCommand { get; }

    public ICommand ImportCommand { get; }

    public ICommand LoadSampleCommand { get; }

    public ICommand RefreshReportCommand { get; }

    public string SelectedFilePath
    {
        get;
        private set
        {
            if (SetProperty(ref field, value))
            {
                ((AsyncRelayCommand)ImportCommand).RaiseCanExecuteChanged();
            }
        }
    } = string.Empty;

    public string StatusMessage
    {
        get;
        private set => SetProperty(ref field, value);
    } = "Vyberte XML soubor nebo načtěte přiložená vzorová data.";

    public bool HasError
    {
        get;
        private set => SetProperty(ref field, value);
    }

    public string TotalWithoutVat => WeekendTotals.Sum(dto => dto.TotalWithoutVat).ToCzechCurrency();

    public string TotalWithVat => WeekendTotals.Sum(dto => dto.TotalWithVat).ToCzechCurrency();

    private async Task BrowseAsync()
    {
        var dialog = new OpenFileDialog
        {
            Title = "Vyberte XML soubor s prodeji vozů",
            Filter = "XML files (*.xml)|*.xml|All files (*.*)|*.*",
            CheckFileExists = true,
            Multiselect = false,
        };

        if (dialog.ShowDialog() != true)
            return;

        SelectedFilePath = dialog.FileName;

        await PreviewXmlAsync(dialog.FileName);
    }

    private async Task LoadSampleAsync()
    {
        var sampleFile = Path.Combine(AppContext.BaseDirectory, "Data", "sales-data.xml");

        if (!File.Exists(sampleFile))
        {
            SetError($"Sample XML file was not found: {sampleFile}");
            return;
        }

        SelectedFilePath = sampleFile;

        await ImportAndRefreshAsync(sampleFile);
    }

    private async Task ImportAsync() => await ImportAndRefreshAsync(SelectedFilePath);

    private async Task PreviewXmlAsync(string filePath)
    {
        var result = await _loadSalesFromXmlQuery.Handle(filePath);

        if (result.IsFailed)
        {
            SetError(result.ToErrorMessage());
            return;
        }

        ReplaceCollection(Sales, result.Value);

        StatusMessage = $"XML náhled načten: {result.Value.Count} záznamů. Klikněte na Importovat do SQLite pro uložení.";

        HasError = false;
    }

    private async Task ImportAndRefreshAsync(string filePath)
    {
        var importResult = await _importSalesFromXmlCommand.Handle(filePath);

        if (importResult.IsFailed)
        {
            SetError(importResult.ToErrorMessage());
            return;
        }

        StatusMessage = $"Import dokončen: {importResult.Value.ImportedCount} záznamů.";
        HasError = false;

        await RefreshAsync();
    }

    private async Task RefreshAsync()
    {
        var allSales = await _getAllCarSalesQuery.Handle();
        if (allSales.IsFailed)
        {
            SetError(allSales.ToErrorMessage());
            return;
        }

        var weekendTotals = await _getWeekendSalesReportQuery.Handle();
        if (weekendTotals.IsFailed)
        {
            SetError(weekendTotals.ToErrorMessage());
            return;
        }

        ReplaceCollection(Sales, allSales.Value);
        ReplaceCollection(WeekendTotals, weekendTotals.Value);

        OnPropertyChanged(nameof(TotalWithoutVat));
        OnPropertyChanged(nameof(TotalWithVat));
    }

    private void SetError(string message)
    {
        HasError = true;
        StatusMessage = message;
        MessageBox.Show(message, "Weekend Car Sales", MessageBoxButton.OK, MessageBoxImage.Warning);
    }

    private static void ReplaceCollection<T>(ObservableCollection<T> collection, IEnumerable<T> values)
    {
        collection.Clear();

        foreach (var value in values)
            collection.Add(value);
    }
}
