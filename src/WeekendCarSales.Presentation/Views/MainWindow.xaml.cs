using WeekendCarSales.Presentation.ViewModels;

namespace WeekendCarSales.Presentation.Views;

public partial class MainWindow
{
    public MainWindow(MainViewModel viewModel)
    {
        InitializeComponent();
        DataContext = viewModel;
    }
}
