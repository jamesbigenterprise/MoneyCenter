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
    private int selectedMonthId;

    [ObservableProperty]
    private ObservableCollection<Expense> recentExpenses = new();

    public DashboardViewModel(IModel model)
    {
        _model = model;
        LoadDashboardData();
    }

    public async void LoadDashboardData()
    {
        try
        {
            CurrentMonthDisplay = DateTime.Now.ToString("MMMM yyyy");

            // Get current month data
            var years = await _model.GetYears();
            var currentYear = years.FirstOrDefault(y => y.YearValue == DateTime.Now.Year);
            
            if (currentYear != null)
            {
                var months = await _model.GetMonthsByYear(currentYear.Id);
                var currentMonth = months.FirstOrDefault(m => m.MonthNumber == DateTime.Now.Month);
                
                if (currentMonth != null)
                {
                    SelectedMonthId = currentMonth.Id;
                    var expenses = await _model.GetExpensesByMonthId(currentMonth.Id);
                    
                    CurrentExpenses = expenses.Sum(e => e.Amount);
                    TotalExpenses = expenses.Sum(e => e.Amount);
                    
                    RecentExpenses.Clear();
                    foreach (var expense in expenses.OrderByDescending(e => e.Date).Take(5))
                    {
                        RecentExpenses.Add(expense.ToViewModel());
                    }
                    
                    // Calculate balance
                    TotalBalance = CurrentIncome - TotalExpenses;
                }
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"LoadDashboardData error: {ex.Message}");
        }
    }

    [RelayCommand]
    public async Task RefreshData()
    {
        LoadDashboardData();
    }
}
