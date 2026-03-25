using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MoneyCenter.Model;
using MoneyCenter.ViewModel.Extensions;
using MoneyCenter.ViewModel.Objects;
using System.Collections.ObjectModel;
using CommunityToolkit.Maui.Alerts;
using SchemaMonth = MoneyCenter.Schema.Month;

namespace MoneyCenter.ViewModel
{
    public partial class MonthViewModel : ObservableObject
    {
        private readonly IModel _model;
        private readonly ObservableCollection<MasterCategory> _masterCategories;
        private int _monthId;

        public MonthViewModel(
            SchemaMonth month,
            IModel model,
            ObservableCollection<Budget> budgets,
            ObservableCollection<MasterCategory> masterCategories)
        {
            _monthId = month.Id;
            MonthName = month.MonthName;
            MonthNumber = month.MonthNumber;
            AllBudgets = budgets;
            _model = model;
            _masterCategories = masterCategories;

            if (!string.IsNullOrEmpty(month.BudgetId))
                SelectedBudget = budgets.FirstOrDefault(b => b.Id == month.BudgetId);
        }

        [ObservableProperty] private string monthName = string.Empty;
        [ObservableProperty] private int monthNumber;
        [ObservableProperty] private bool isExpanded;
        [ObservableProperty] private int transactionCount;
        [ObservableProperty] private Budget selectedBudget;
        [ObservableProperty] private ObservableCollection<Expense> expenses = new();
        [ObservableProperty] private ObservableCollection<Budget> allBudgets;

        partial void OnIsExpandedChanged(bool value)
        {
            if (value && !Expenses.Any())
                Task.Run(LoadExpensesAsync);
        }

        partial void OnSelectedBudgetChanged(Budget value)
        {
            if (value != null)
                _ = _model.AssignBudgetToMonth(_monthId, value.Id);
        }

        public async Task LoadExpensesAsync()
        {
            var schemaExpenses = await _model.GetExpensesByMonthId(_monthId);
            Expenses.Clear();
            foreach (var expense in schemaExpenses)
            {
                var vm = expense.ToViewModel();
                var category = _masterCategories.FirstOrDefault(c => c.Id == expense.MasterCategoryId);
                vm.CategoryName = category?.Name ?? string.Empty;
                var capturedId = expense.Id;
                vm.DeleteCommand = new AsyncRelayCommand(() => DeleteExpenseAsync(capturedId));
                Expenses.Add(vm);
            }
            TransactionCount = Expenses.Count;
        }

        [RelayCommand]
        private void ToggleExpand() => IsExpanded = !IsExpanded;

        [RelayCommand]
        private async Task AddExpense()
        {
            var newEntryView = IPlatformApplication.Current.Services.GetRequiredService<Views.NewEntryView>();
            if (newEntryView.BindingContext is NewEntryViewModel vm)
                vm.NewEntryModel.MonthId = _monthId;
            await Application.Current!.MainPage!.Navigation.PushModalAsync(newEntryView);
        }

        private async Task DeleteExpenseAsync(string expenseId)
        {
            await _model.DeleteExpense(_monthId, expenseId);
            await LoadExpensesAsync();
            await Toast.Make("Expense deleted from ledger.", CommunityToolkit.Maui.Core.ToastDuration.Short).Show();
        }
    }
}
