using MoneyCenter.ViewModel;

namespace MoneyCenter.Views;

public partial class ExpensesView : ContentView
{
    public ExpensesView(ExpensesViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
}