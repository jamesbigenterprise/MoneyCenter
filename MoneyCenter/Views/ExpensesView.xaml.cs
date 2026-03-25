using MoneyCenter.ViewModel;

namespace MoneyCenter.Views;

public partial class ExpensesView : ContentView
{
    public ExpensesView(ExpensesViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }

    protected override void OnPropertyChanged(string propertyName = null)
    {
        base.OnPropertyChanged(propertyName);
        if (propertyName == nameof(IsVisible) && IsVisible && BindingContext is ExpensesViewModel vm)
            vm.LoadData();
    }
}