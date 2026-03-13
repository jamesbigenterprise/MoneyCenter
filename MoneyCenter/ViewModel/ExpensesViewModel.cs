using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MoneyCenter.Model;
using MoneyCenter.Services;
using MoneyCenter.ViewModel.Extensions;
using MoneyCenter.ViewModel.Objects;
using System.Collections.ObjectModel;

namespace MoneyCenter.ViewModel;

public partial class ExpensesViewModel : ObservableObject
{
    private readonly IModel _model;
    private readonly IToastService _toastService;

    public ExpensesViewModel(IModel model, IToastService toastService)
    {
        _model = model;
        _toastService = toastService;
        LoadData();
    }

    [ObservableProperty]
    private int selectedYearId;

    [ObservableProperty]
    private int selectedMonthId;

    [ObservableProperty]
    private ObservableCollection<Year> years = new();

    [ObservableProperty]
    private ObservableCollection<Month> months = new();

    [ObservableProperty]
    private ObservableCollection<Budget> budgets = new();

    [ObservableProperty]
    private Budget selectedBudget;

    [ObservableProperty]
    private ObservableCollection<MasterCategory> masterCategories = new();

    [ObservableProperty]
    private ObservableCollection<PaymentAccount> paymentAccounts = new();

    [ObservableProperty]
    private ObservableCollection<Expense> currentExpenses = new();

    [ObservableProperty]
    private decimal newExpenseAmount;

    [ObservableProperty]
    private DateTime newExpenseDate = DateTime.Today;

    [ObservableProperty]
    private string newExpenseDestination;

    [ObservableProperty]
    private string newExpenseDetails;

    [ObservableProperty]
    private int selectedCategoryId;

    [ObservableProperty]
    private int selectedPaymentAccountId;

    [ObservableProperty]
    private Expense editingExpense;

    [ObservableProperty]
    private bool isEditing;

    public async void LoadData()
    {
        try
        {
            var yearsList = await _model.GetYears();
            Years.Clear();
            foreach (var year in yearsList)
            {
                Years.Add(new Year { Id = year.Id, YearValue = year.YearValue });
            }

            var budgetsList = await _model.GetBudgets();
            Budgets.Clear();
            foreach (var budget in budgetsList)
            {
                Budgets.Add(budget.ToViewModel());
            }

            var categoriesList = await _model.GetMasterCategories();
            MasterCategories.Clear();
            foreach (var category in categoriesList)
            {
                MasterCategories.Add(category.ToViewModel());
            }

            var accountsList = await _model.GetPaymentAccounts();
            PaymentAccounts.Clear();
            foreach (var account in accountsList)
            {
                PaymentAccounts.Add(account.ToViewModel());
            }

            // Set to current year and month
            if (Years.Any())
            {
                SelectedYearId = Years.First().Id;
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"LoadData error: {ex.Message}");
        }
    }

    partial void OnSelectedYearIdChanged(int yearId)
    {
        if (yearId > 0)
        {
            LoadMonthsForYear();
        }
    }

    private async void LoadMonthsForYear()
    {
        try
        {
            var monthsList = await _model.GetMonthsByYear(SelectedYearId);
            Months.Clear();
            foreach (var month in monthsList)
            {
                Months.Add(new Month { Id = month.Id, YearId = month.YearId, BudgetId = month.BudgetId, MonthNumber = month.MonthNumber, MonthName = month.MonthName });
            }

            if (Months.Any())
            {
                SelectedMonthId = Months.First(m => m.MonthNumber == DateTime.Now.Month)?.Id ?? Months.First().Id;
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"LoadMonthsForYear error: {ex.Message}");
        }
    }

    partial void OnSelectedMonthIdChanged(int monthId)
    {
        if (monthId > 0)
        {
            Task.Run(async () => await LoadExpensesForMonth());
        }
    }

    private async Task LoadExpensesForMonth()
    {
        try
        {
            // Load expenses for the selected month
            var expenses = await _model.GetExpensesByMonthId(SelectedMonthId);
            CurrentExpenses.Clear();
            foreach (var expense in expenses.OrderByDescending(e => e.Date))
            {
                CurrentExpenses.Add(expense.ToViewModel());
            }

            // Load the budget assigned to this month
            var budget = await _model.GetBudgetForMonth(SelectedMonthId);
            if (budget != null)
            {
                SelectedBudget = budget.ToViewModel();
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"LoadExpensesForMonth error: {ex.Message}");
        }
    }

    [RelayCommand]
    private async Task AddExpense()
    {
        if (SelectedMonthId <= 0 || SelectedCategoryId <= 0 || SelectedPaymentAccountId <= 0)
        {
            await App.Current.MainPage.DisplayAlert("Validation", "Please select month, category, and payment account", "OK");
            return;
        }

        var expense = new Schema.Expense
        {
            Id = Guid.NewGuid().ToString(),
            MonthId = SelectedMonthId,
            Amount = NewExpenseAmount,
            Date = NewExpenseDate,
            MasterCategoryId = SelectedCategoryId,
            Destination = NewExpenseDestination,
            Details = NewExpenseDetails,
            PaymentAccountId = SelectedPaymentAccountId
        };

        try
        {
            await _model.AddExpense(SelectedMonthId, expense);
            await LoadExpensesForMonth();

            // Clear form
            NewExpenseAmount = 0;
            NewExpenseDestination = string.Empty;
            NewExpenseDetails = string.Empty;
            NewExpenseDate = DateTime.Today;

            System.Diagnostics.Debug.WriteLine("Expense added successfully");
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"AddExpense error: {ex.Message}");
        }
    }

    [RelayCommand]
    private async Task DeleteExpense(string expenseId)
    {
        try
        {
            await _model.DeleteExpense(SelectedMonthId, expenseId);
            await LoadExpensesForMonth();
            System.Diagnostics.Debug.WriteLine("Expense deleted");
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"DeleteExpense error: {ex.Message}");
        }
    }
}
