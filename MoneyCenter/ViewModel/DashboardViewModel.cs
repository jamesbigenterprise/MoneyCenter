using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using MoneyCenter.Model;
using MoneyCenter.ViewModel.Extensions;
using MoneyCenter.ViewModel.Objects;

namespace MoneyCenter.ViewModel;

public partial class DashboardViewModel : ObservableObject
{
    private readonly IModel _model;

    [ObservableProperty]
    private string currentMonthDisplay;

    [ObservableProperty]
    private decimal currentIncome;

    [ObservableProperty]
    private decimal currentExpenses;

    [ObservableProperty]
    private decimal totalIncome;

    [ObservableProperty]
    private decimal totalExpenses;

    [ObservableProperty]
    private decimal totalBalance;

    [ObservableProperty]
    private ObservableCollection<Expense> recentExpenses = new();

    public DashboardViewModel(IModel model)
    {
        _model = model;

        // Initialize the view with data
        LoadDashboardData();
    }

    public async void LoadDashboardData()
    {
        ///TODO
        
       
   
    }

    partial void OnCurrentIncomeChanged(decimal value)
    {
        // Update the income for the current month
        _model.SetIncomeForMonth(value, DateTime.Now.ToString("yyyy-MM"));
    }

    [RelayCommand]
    public async Task AddNewMonth()
    {
        await _model.AddNewMonth();
        LoadDashboardData();
    }
}
