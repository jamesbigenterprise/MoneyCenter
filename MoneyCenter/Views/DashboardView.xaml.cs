using MoneyCenter.ViewModel;
namespace MoneyCenter;

public partial class DashboardView : ContentView
{
    public DashboardView(DashboardViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
}
