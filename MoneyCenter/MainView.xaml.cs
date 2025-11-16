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

        // Subscribe to view changes
        _viewModel.PropertyChanged += ViewModelPropertyChanged;

        // Set initial view
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
        MainContent.Content = activeView switch
        {
            "dashboard" => _dashboardView.Content,
            "expenses" => _expensesView.Content,
            "savings" => _savingsView.Content,
            "budgets" => _budgetsView.Content,
            "settings" => _settingsView.Content,
            _ => _dashboardView.Content
        };
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        // Check device screen size and update IsDesktop/IsMobile
        _viewModel.CheckDeviceSize();
    }

    protected override void OnSizeAllocated(double width, double height)
    {
        base.OnSizeAllocated(width, height);
        _viewModel.CheckDeviceSize();
    }
}
