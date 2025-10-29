using MoneyCenter.ViewModel;

namespace MoneyCenter.Views;

public partial class SavingsView : ContentView
{
    public SavingsView(SavingsViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
}