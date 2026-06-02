using MoneyCenter.ViewModel;

namespace MoneyCenter.Views;

public partial class BudgetsView : ContentView
{
    public BudgetsView(BudgetsViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }

    protected override void OnPropertyChanged(string propertyName = null)
    {
        base.OnPropertyChanged(propertyName);
        if (propertyName == nameof(IsVisible) && IsVisible && BindingContext is BudgetsViewModel vm)
        {
            _ = vm.LoadDataAsync();
        }
    }
}