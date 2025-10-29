using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using MoneyCenter.Services;
using MoneyCenter.ViewModel.Objects;

namespace MoneyCenter.ViewModel;

public partial class DashboardViewModel : ObservableObject
{
    private readonly IFinancialService _financialService;

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

    public DashboardViewModel(IFinancialService financialService)
    {
        _financialService = financialService;

        // Initialize the view with data
        LoadDashboardData();
    }

    public void LoadDashboardData()
    {
        var currentMonthKey = DateTime.Now.ToString("yyyy-MM");
        var allData = _financialService.GetAllData();
        var currentData = allData.ContainsKey(currentMonthKey) ?
            allData[currentMonthKey] :
            new MonthlyData { Income = 0, Expenses = new List<Expense>() };

        CurrentMonthDisplay = DateTime.Parse($"{currentMonthKey}-01").ToString("MMMM yyyy");
        CurrentIncome = currentData.Income;
        CurrentExpenses = currentData.Expenses.Sum(e => e.Amount);

        // Calculate totals across all data
        TotalIncome = allData.Sum(d => d.Value.Income);
        TotalExpenses = allData.Sum(d => d.Value.Expenses.Sum(e => e.Amount));
        TotalBalance = TotalIncome - TotalExpenses;

        // Get recent expenses
        var allExpenses = allData
            .SelectMany(d => d.Value.Expenses)
            .OrderByDescending(e => e.Date)
            .Take(5)
            .ToList();

        RecentExpenses.Clear();
        foreach (var expense in allExpenses)
        {
            RecentExpenses.Add(expense);
        }
    }

    partial void OnCurrentIncomeChanged(decimal value)
    {
        // Update the income for the current month
        _financialService.SetIncomeForMonth(value, DateTime.Now.ToString("yyyy-MM"));
    }

    [RelayCommand]
    public async Task AddNewMonth()
    {
        await _financialService.AddNewMonth();
        LoadDashboardData();
    }
}
