using CommunityToolkit.Maui.ApplicationModel;
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

        // Initialize data
        LoadData();
    }

    [ObservableProperty]
    private ObservableCollection<string> availableMonths = new();

    [ObservableProperty]
    private string selectedMonth;

    [ObservableProperty]
    private ObservableCollection<Budget> budgets = new();

    [ObservableProperty]
    private Budget selectedBudget;

    [ObservableProperty]
    private ObservableCollection<string> categories = new();

    [ObservableProperty]
    private string selectedCategory;

    [ObservableProperty]
    private ObservableCollection<Expense> currentExpenses = new();

    // New expense properties
    [ObservableProperty]
    private decimal newExpenseAmount;

    [ObservableProperty]
    private DateTime newExpenseDate = DateTime.Today;

    [ObservableProperty]
    private string newExpenseDestination;

    [ObservableProperty]
    private string newExpenseDetails;

    // Expense being edited
    [ObservableProperty]
    private Expense editingExpense;

    [ObservableProperty]
    private bool isEditing;

    public async void LoadData()
    {
        var schemaAllData = await _model.GetAllData();
        var allExpenses = await _model.GetAllExpenses();
        var allData = schemaAllData.ToDictionary(
            kvp => kvp.Key, 
            kvp => kvp.Value.ToViewModel(allExpenses));
        var monthList = allData.Keys.OrderByDescending(k => k).ToList();

        // Update available months
        AvailableMonths.Clear();
        foreach (var month in monthList)
        {
            AvailableMonths.Add(month);
        }

        // Set current month if not already set
        if (string.IsNullOrEmpty(SelectedMonth) && AvailableMonths.Count > 0)
        {
            SelectedMonth = AvailableMonths[0];
        }

        // Load budgets
        var schemaBudgets = await _model.GetBudgets();
        var allCategories = await _model.GetMasterCategories();
        var budgetList = schemaBudgets.Select(b => b.ToViewModel(allCategories)).ToList();
        Budgets.Clear();
        foreach (var budget in budgetList)
        {
            Budgets.Add(budget);
        }

        // Select the budget for this month
        if (!string.IsNullOrEmpty(SelectedMonth) && allData.ContainsKey(SelectedMonth))
        {
            var monthData = allData[SelectedMonth];
            var monthBudget = budgetList.FirstOrDefault(b => b.Id == monthData.BudgetId);
            if (monthBudget != null)
            {
                SelectedBudget = monthBudget;
            }
        }

        // Load categories
        var categoryNames = await _model.GetAllCategories();
        Categories.Clear();
        foreach (var category in categoryNames)
        {
            Categories.Add(category);
        }

        // Load current month's expenses
        await LoadExpenses();
    }

    partial void OnSelectedMonthChanged(string value)
    {
        if (!string.IsNullOrEmpty(value))
        {
            Task.Run(async () => await LoadExpensesAndUpdateBudget());
        }
    }

    private async Task LoadExpensesAndUpdateBudget()
    {
        await LoadExpenses();

        // Update selected budget
        var schemaAllData = await _model.GetAllData();
        var allExpenses = await _model.GetAllExpenses();
        var allData = schemaAllData.ToDictionary(
            kvp => kvp.Key, 
            kvp => kvp.Value.ToViewModel(allExpenses));
        
        if (allData.ContainsKey(SelectedMonth))
        {
            var monthData = allData[SelectedMonth];
            var monthBudget = Budgets.FirstOrDefault(b => b.Id == monthData.BudgetId);
            if (monthBudget != null)
            {
                SelectedBudget = monthBudget;
            }
        }
    }

    partial void OnSelectedBudgetChanged(Budget value)
    {
        if (value != null && !string.IsNullOrEmpty(SelectedMonth))
        {
            _model.SetBudgetForMonth(value.Id, SelectedMonth);
        }
    }

    private async Task LoadExpenses()
    {
        if (string.IsNullOrEmpty(SelectedMonth))
            return;

        var monthExpenses = await _model.GetExpensesByMonth(SelectedMonth);
        var expenses = monthExpenses.Select(e => e.ToViewModel()).ToList();
        
        CurrentExpenses.Clear();
        foreach (var expense in expenses.OrderByDescending(e => e.Date))
        {
            CurrentExpenses.Add(expense);
        }
    }

    [RelayCommand]
    private void PreviousMonth()
    {
        var index = AvailableMonths.IndexOf(SelectedMonth);
        if (index < AvailableMonths.Count - 1)
        {
            SelectedMonth = AvailableMonths[index + 1];
        }
    }

    [RelayCommand]
    private void NextMonth()
    {
        var index = AvailableMonths.IndexOf(SelectedMonth);
        if (index > 0)
        {
            SelectedMonth = AvailableMonths[index - 1];
        }
    }

    [RelayCommand]
    private async Task AddCategory()
    {
        string result = await App.Current.MainPage.DisplayPromptAsync("New Category", "Enter category name:", "Add", "Cancel");

        if (!string.IsNullOrWhiteSpace(result))
        {
            // Add new category to master list
            var newCategory = new BudgetCategory
            {
                Name = result,
                Amount = 0,
                Type = "expense",
                IsRecurring = false
            };
            
            await _model.AddCategoryAsync(newCategory.ToSchema());

            // Reload categories
            var allCategories = await _model.GetAllCategories();
            Categories.Clear();
            foreach (var category in allCategories)
            {
                Categories.Add(category);
            }

            // Select the new category
            SelectedCategory = result;
        }
    }

    [RelayCommand]
    private async Task AddExpense()
    {
        if (NewExpenseAmount <= 0)
        {
            _toastService.Show("Amount must be greater than zero");
            return;
        }

        if (string.IsNullOrWhiteSpace(SelectedCategory))
        {
            _toastService.Show("Please select a category");
            return;
        }

        if (string.IsNullOrWhiteSpace(NewExpenseDestination))
        {
            _toastService.Show("Please enter a destination");
            return;
        }

        var expense = new Expense
        {
            Id = Guid.NewGuid().ToString(),
            Amount = NewExpenseAmount,
            Date = NewExpenseDate,
            Category = SelectedCategory,
            Destination = NewExpenseDestination,
            Details = NewExpenseDetails
        };

        if (IsEditing && EditingExpense != null)
        {
            // Update existing expense
            expense.Id = EditingExpense.Id;
            await _model.UpdateExpense(SelectedMonth, expense.ToSchema());
            _toastService.Show("Expense updated");
        }
        else
        {
            // Add new expense
            await _model.AddExpense(SelectedMonth, expense.ToSchema());
            _toastService.Show("Expense added");
        }

        // Reset form
        ResetForm();

        // Reload expenses
        await LoadExpenses();
    }

    [RelayCommand]
    private void EditExpense(Expense expense)
    {
        if (expense == null)
            return;

        EditingExpense = expense;
        IsEditing = true;

        // Populate form with expense data
        NewExpenseAmount = expense.Amount;
        NewExpenseDate = expense.Date;
        SelectedCategory = expense.Category;
        NewExpenseDestination = expense.Destination;
        NewExpenseDetails = expense.Details;
    }

    [RelayCommand]
    private async Task DeleteExpense(Expense expense)
    {
        if (expense == null)
            return;

        bool confirm = await App.Current.MainPage.DisplayAlert("Confirm",
            "Are you sure you want to delete this expense?", "Yes", "No");

        if (confirm)
        {
            await _model.DeleteExpense(SelectedMonth, expense.Id);
            _toastService.Show("Expense deleted");
            await LoadExpenses();
        }
    }

    private void ResetForm()
    {
        NewExpenseAmount = 0;
        NewExpenseDate = DateTime.Today;
        SelectedCategory = null;
        NewExpenseDestination = string.Empty;
        NewExpenseDetails = string.Empty;
        EditingExpense = null;
        IsEditing = false;
    }
}
