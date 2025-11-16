using CommunityToolkit.Maui.ApplicationModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MoneyCenter.Services;
using MoneyCenter.ViewModel.Objects;
using System.Collections.ObjectModel;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace MoneyCenter.ViewModel;

public partial class MainViewModel : ObservableObject
{
    private readonly IFinancialService _financialService;
    private readonly IDeviceDisplay _deviceDisplay;

    public MainViewModel(IFinancialService financialService, IDeviceDisplay deviceDisplay)
    {
        _financialService = financialService;
        _deviceDisplay = deviceDisplay;

        // Initialize with dashboard view
        ActiveView = "dashboard";

        // Initialize data
        InitializeData();
    }

    [ObservableProperty]
    private string activeView;

    [ObservableProperty]
    private bool isDesktop;

    [ObservableProperty]
    private bool isMobile;

    [ObservableProperty]
    private Dictionary<string, MonthlyData> data = new();

    [ObservableProperty]
    private string currentMonth;

    [ObservableProperty]
    private List<Budget> budgets = new();

    [ObservableProperty]
    private List<int> visibleYears = new();

    [ObservableProperty]
    private List<BudgetCategory> masterCategories = new();

    public void CheckDeviceSize()
    {
        var screenWidth = _deviceDisplay.MainDisplayInfo.Width / _deviceDisplay.MainDisplayInfo.Density;
        IsDesktop = screenWidth >= 1024;
        IsMobile = !IsDesktop;
    }

    [RelayCommand]
    private void Navigate(string view)
    {
        ActiveView = view;
    }

    private void InitializeData()
    {
        currentMonth = DateTime.Now.ToString("yyyy-MM");
        visibleYears = new List<int> { DateTime.Now.Year };

        // Initialize with default data from service
        budgets = _financialService.GetInitialBudgets();
        masterCategories = _financialService.GetMasterCategories();
        data = _financialService.InitializeYearData(DateTime.Now.Year);
    }

    [RelayCommand]
    private void AddNewMonth()
    {
        // Get the last month in the data
        var lastMonth = Data.Keys.OrderBy(k => k).LastOrDefault() ?? CurrentMonth;
        var lastDate = DateTime.Parse($"{lastMonth}-01");
        var newMonthDate = lastDate.AddMonths(1);
        var newMonth = newMonthDate.ToString("yyyy-MM");

        var defaultBudget = Budgets.FirstOrDefault(b => b.IsDefault);
        var incomeFromBudget = defaultBudget?.Categories.FirstOrDefault(c => c.Type == "income")?.Amount ?? 5000;

        // Add the new month to data
        var initialData = new MonthlyData
        {
            Income = incomeFromBudget,
            Expenses = new List<Expense>(),
            BudgetId = defaultBudget?.Id
        };

        Data[newMonth] = initialData;

        // Check if we need to add a new year
        var newYear = newMonthDate.Year;
        if (!VisibleYears.Contains(newYear))
        {
            VisibleYears.Add(newYear);
            VisibleYears.Sort();
        }

        CurrentMonth = newMonth;
    }

    [RelayCommand]
    private void AddYear(string direction)
    {
        int currentEdgeYear = direction == "prev" ? VisibleYears.Min() : VisibleYears.Max();
        int newYear = direction == "prev" ? currentEdgeYear - 1 : currentEdgeYear + 1;

        if (!VisibleYears.Contains(newYear))
        {
            // Add the year's data
            var yearData = _financialService.InitializeYearData(newYear);
            foreach (var kvp in yearData)
            {
                Data[kvp.Key] = kvp.Value;
            }

            VisibleYears.Add(newYear);
            VisibleYears.Sort();
        }
    }

    [RelayCommand]
    private void ShowToast(string message)
    {
        // Implementation will depend on the MAUI toast service you're using
        // For example:
        // _toastService.Show(message);
    }
}
