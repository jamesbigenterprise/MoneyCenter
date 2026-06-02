using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MoneyCenter.Model;
using MoneyCenter.Services;
using MoneyCenter.ViewModel.Extensions;
using MoneyCenter.ViewModel.Objects;
using System.Collections.ObjectModel;
using CommunityToolkit.Maui.Alerts;

namespace MoneyCenter.ViewModel;

public partial class ExpensesViewModel : ObservableObject
{
    private readonly IModel _model;
    private bool _isLoading;

    public ExpensesViewModel(IModel model, IToastService toastService)
    {
        _model = model;
    }

    [ObservableProperty]
    private ObservableCollection<YearViewModel> years = new();

    [ObservableProperty]
    private ObservableCollection<Budget> budgets = new();

    [ObservableProperty]
    private ObservableCollection<MasterCategory> masterCategories = new();

    [ObservableProperty]
    private ObservableCollection<PaymentAccount> paymentAccounts = new();

    public async void LoadData()
    {
        if (_isLoading) return;
        _isLoading = true;
        try
        {
            var budgetsList = await _model.GetBudgets();
            var categoriesList = await _model.GetMasterCategories();
            var accountsList = await _model.GetPaymentAccounts();
            var schemaYears = await _model.GetYears();

            if (!schemaYears.Any())
            {
                await _model.AddYear(DateTime.Now.Year);
                schemaYears = await _model.GetYears();
            }

            MainThread.BeginInvokeOnMainThread(() =>
            {
                Budgets.Clear();
                foreach (var b in budgetsList)
                    Budgets.Add(b.ToViewModel());

                MasterCategories.Clear();
                foreach (var c in categoriesList)
                    MasterCategories.Add(c.ToViewModel());

                PaymentAccounts.Clear();
                foreach (var a in accountsList)
                    PaymentAccounts.Add(a.ToViewModel());

                Years.Clear();
                foreach (var year in schemaYears.OrderByDescending(y => y.YearValue))
                    Years.Add(new YearViewModel(year, _model, Budgets, MasterCategories));
            });
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"LoadData error: {ex.Message}");
        }
        finally
        {
            _isLoading = false;
        }
    }

    private async Task LoadYearsAsync()
    {
        var schemaYears = await _model.GetYears();
        var years = schemaYears.OrderByDescending(y => y.YearValue).ToList();
        MainThread.BeginInvokeOnMainThread(() =>
        {
            Years.Clear();
            foreach (var year in years)
                Years.Add(new YearViewModel(year, _model, Budgets, MasterCategories));
        });
    }

    [RelayCommand]
    private async Task AddPreviousYear()
    {
        int prevYear = Years.Any()
            ? Years.Min(y => y.YearValue) - 1
            : DateTime.Now.Year - 1;
        await _model.AddYear(prevYear);
        await LoadYearsAsync();
        await Toast.Make($"Added year {prevYear} to ledger.", CommunityToolkit.Maui.Core.ToastDuration.Short).Show();
    }

    [RelayCommand]
    private async Task AddNextYear()
    {
        int nextYear = Years.Any()
            ? Years.Max(y => y.YearValue) + 1
            : DateTime.Now.Year + 1;
        await _model.AddYear(nextYear);
        await LoadYearsAsync();
        await Toast.Make($"Added year {nextYear} to ledger.", CommunityToolkit.Maui.Core.ToastDuration.Short).Show();
    }
}
