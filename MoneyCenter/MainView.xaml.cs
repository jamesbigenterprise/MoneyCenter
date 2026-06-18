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
    private View? _activeContent;
    private bool _isSwitching;

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
        foreach (var child in MainContent.Children.OfType<View>())
            child.IsVisible = false;

        _ = UpdateActiveView(_viewModel.ActiveView, false);

        _viewModel.PropertyChanged += ViewModelPropertyChanged;
    }

    private void OnPageLoaded(object sender, EventArgs e)
    {
        _ = UpdateActiveView(_viewModel.ActiveView, false);
    }

    private void ViewModelPropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(MainViewModel.ActiveView))
        {
            _ = UpdateActiveView(_viewModel.ActiveView);
        }
    }

    private async Task UpdateActiveView(string activeView, bool animate = true)
    {
        if (_isSwitching)
            return;

        var nextView = GetContentView(activeView);
        if (nextView == null || ReferenceEquals(nextView, _activeContent))
            return;

        _isSwitching = true;

        if (_activeContent != null && animate)
        {
            await Task.WhenAll(
                _activeContent.FadeToAsync(0, 100, Easing.CubicOut),
                _activeContent.TranslateToAsync(-8, 0, 100, Easing.CubicOut));
            _activeContent.IsVisible = false;
        }
        else if (_activeContent != null)
        {
            _activeContent.IsVisible = false;
        }

        nextView.Opacity = animate ? 0 : 1;
        nextView.TranslationX = animate ? 8 : 0;
        nextView.IsVisible = true;

        if (animate)
        {
            await Task.WhenAll(
                nextView.FadeToAsync(1, 130, Easing.CubicOut),
                nextView.TranslateToAsync(0, 0, 130, Easing.CubicOut));
        }

        _activeContent = nextView;
        _isSwitching = false;

        if (activeView == "expenses")
        {
            if (_expensesView.BindingContext is ExpensesViewModel expVm) expVm.LoadData();
        }
        else if (activeView == "budgets")
        {
            if (_budgetsView.BindingContext is BudgetsViewModel budVm) _ = budVm.LoadDataAsync();
        }
    }

    private View? GetContentView(string activeView)
    {
        switch (activeView)
        {
            case "dashboard":
                return _dashboardView;
            case "expenses":
                return _expensesView;
            case "savings":
                return _savingsView;
            case "budgets":
                return _budgetsView;
            case "settings":
                return _settingsView;
            default:
                return null;
        }
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
