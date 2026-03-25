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

        Budgets = new();
        Years = new();
        MasterCategories = new();
        PaymentAccounts = new();

        ActiveView = "dashboard";
        InitializeData();
    }

    [ObservableProperty]
    public partial string ActiveView { get; set; }

    [ObservableProperty]
    public partial bool IsDesktop { get; set; }

    [ObservableProperty]
    public partial bool IsMobile { get; set; }

    [ObservableProperty]
    public partial string CurrentMonth { get; set; }

    [ObservableProperty]
    public partial List<Budget> Budgets { get; set; }

    [ObservableProperty]
    public partial List<Year> Years { get; set; }

    [ObservableProperty]
    public partial List<MasterCategory> MasterCategories { get; set; }

    [ObservableProperty]
    public partial List<PaymentAccount> PaymentAccounts { get; set; }

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
        try
        {
            CurrentMonth = DateTime.Now.ToString("yyyy-MM");

            // Load all base data
            var schemaBudgets = await _model.GetBudgets();
            Budgets = schemaBudgets.Select(b => b.ToViewModel()).ToList();

            var schemaYears = await _model.GetYears();
            Years = schemaYears.Select(y => new Year { Id = y.Id, YearValue = y.YearValue }).ToList();

            var schemaCategories = await _model.GetMasterCategories();
            MasterCategories = schemaCategories.Select(c => c.ToViewModel()).ToList();

            var schemaAccounts = await _model.GetPaymentAccounts();
            PaymentAccounts = schemaAccounts.Select(a => a.ToViewModel()).ToList();

            // Seed initial data if needed
            await _model.SeedInitialData();
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"InitializeData error: {ex.Message}");
        }
    }

    [RelayCommand]
    private async Task AddYear(int year)
    {
        try
        {
            await _model.AddYear(year);
            var schemaYears = await _model.GetYears();
            Years = schemaYears.Select(y => new Year { Id = y.Id, YearValue = y.YearValue }).ToList();
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"AddYear error: {ex.Message}");
        }
    }
}
