using MoneyCenter.ViewModel;

namespace MoneyCenter.Views;

public partial class BudgetsView : ContentView
{
    public BudgetsView(BudgetsViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
}