using MoneyCenter.ViewModel;
using MoneyCenter.Views;
namespace MoneyCenter;

public partial class MainView : ContentPage
{
    private readonly MainViewModel _viewModel;
    private readonly DashboardView _dashboardView;
    private readonly ExpensesView _expensesView;
    private readonly SavingsView _savingsView;
    private readonly BudgetsView _budgetsView;
    private readonly SettingsView _settingsView;

    public MainView(MainViewModel viewModel,
                   DashboardView dashboardView,
                   ExpensesView expensesView,
                   SavingsView savingsView,
                   BudgetsView budgetsView,
                   SettingsView settingsView)
    {
        InitializeComponent();
        _viewModel = viewModel;
        BindingContext = _viewModel;

        _dashboardView = dashboardView;
        _expensesView = expensesView;
        _savingsView = savingsView;
        _budgetsView = budgetsView;
        _settingsView = settingsView;

        MainContent.Children.Add(_dashboardView);
        MainContent.Children.Add(_expensesView);
        MainContent.Children.Add(_savingsView);
        MainContent.Children.Add(_budgetsView);
        MainContent.Children.Add(_settingsView);

        UpdateActiveView(_viewModel.ActiveView);

        _viewModel.PropertyChanged += ViewModelPropertyChanged;
    }

    private void OnPageLoaded(object sender, EventArgs e)
    {
        UpdateActiveView(_viewModel.ActiveView);
    }

    private void ViewModelPropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(MainViewModel.ActiveView))
        {
            UpdateActiveView(_viewModel.ActiveView);
        }
    }

    private void UpdateActiveView(string activeView)
    {
        _dashboardView.IsVisible = activeView == "dashboard";
        _expensesView.IsVisible = activeView == "expenses";
        _savingsView.IsVisible = activeView == "savings";
        _budgetsView.IsVisible = activeView == "budgets";
        _settingsView.IsVisible = activeView == "settings";
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        _viewModel.CheckDeviceSize();
    }

    protected override void OnSizeAllocated(double width, double height)
    {
        base.OnSizeAllocated(width, height);
        _viewModel.CheckDeviceSize();
    }
}
