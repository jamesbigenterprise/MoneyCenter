using CommunityToolkit.Maui.ApplicationModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MoneyCenter.Model;
using MoneyCenter.ViewModel.Extensions;
using MoneyCenter.ViewModel.Objects;
using System.Collections.ObjectModel;

namespace MoneyCenter.ViewModel;

public partial class MainViewModel : ObservableObject
{
    private readonly IModel _model;
    private readonly IDeviceDisplay _deviceDisplay;

    public MainViewModel(IModel model, IDeviceDisplay deviceDisplay)
    {
        _model = model;
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

    private async void InitializeData()
    {
        currentMonth = DateTime.Now.ToString("yyyy-MM");
        visibleYears = new List<int> { DateTime.Now.Year };

        // Initialize with default data from model
        var schemaBudgets = await _model.GetBudgets();
        var allCategories = await _model.GetMasterCategories();
        budgets = schemaBudgets.Select(b => b.ToViewModel(allCategories)).ToList();
        
        var schemaCategories = await _model.GetMasterCategories();
        masterCategories = schemaCategories.Select(c => c.ToViewModel()).ToList();
        
        var schemaData = await _model.GetAllData();
        var allExpenses = await _model.GetAllExpenses();
        data = schemaData
            .Where(kvp => kvp.Key.StartsWith(DateTime.Now.Year.ToString()))
            .ToDictionary(kvp => kvp.Key, kvp => kvp.Value.ToViewModel(allExpenses));
    }

    [RelayCommand]
    private async Task AddNewMonth()
    {
        // Get the last month in the data
        var lastMonth = Data.Keys.OrderBy(k => k).LastOrDefault() ?? CurrentMonth;
        var lastDate = DateTime.Parse($"{lastMonth}-01");
        var newMonthDate = lastDate.AddMonths(1);
        var newMonth = newMonthDate.ToString("yyyy-MM");

        var defaultBudget = Budgets.FirstOrDefault(b => b.IsDefault);
        var incomeFromBudget = defaultBudget?.Categories.FirstOrDefault(c => c.Type == "income")?.Amount ?? 5000;

        // Add the new month via model
        await _model.AddNewMonth();

        // Reload the data
        var schemaData = await _model.GetAllData();
        var allExpenses = await _model.GetAllExpenses();
        data = schemaData.ToDictionary(kvp => kvp.Key, kvp => kvp.Value.ToViewModel(allExpenses));

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
    private async Task AddYear(string direction)
    {
        int currentEdgeYear = direction == "prev" ? VisibleYears.Min() : VisibleYears.Max();
        int newYear = direction == "prev" ? currentEdgeYear - 1 : currentEdgeYear + 1;

        if (!VisibleYears.Contains(newYear))
        {
            // Get all data including the new year
            var schemaData = await _model.GetAllData();
            var allExpenses = await _model.GetAllExpenses();
            var allData = schemaData.ToDictionary(kvp => kvp.Key, kvp => kvp.Value.ToViewModel(allExpenses));
            
            // Filter for the new year's data
            var yearData = allData.Where(kvp => kvp.Key.StartsWith(newYear.ToString()));
            
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
