using CommunityToolkit.Mvvm.ComponentModel;
using MoneyCenter.Model;
using MoneyCenter.Services;
using MoneyCenter.ViewModel.Extensions;
using MoneyCenter.ViewModel.Objects;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoneyCenter.ViewModel
{
    public partial class SavingsViewModel : ObservableObject
    {
        private readonly IModel _model;

        public SavingsViewModel(IModel model)
        {
            _model = model;
            LoadSavings();
        }

        [ObservableProperty]
        private decimal totalSavings;

        [ObservableProperty]
        private ObservableCollection<BudgetCategory> savingsCategories = new();

        private async void LoadSavings()
        {
            var schemaBudgets = await _model.GetBudgets();
            var allCategories = await _model.GetMasterCategories();
            var budgets = schemaBudgets.Select(b => b.ToViewModel(allCategories)).ToList();
            
            var schemaAllData = await _model.GetAllData();
            var allExpenses = await _model.GetAllExpenses();
            var allData = schemaAllData.ToDictionary(
                kvp => kvp.Key, 
                kvp => kvp.Value.ToViewModel(allExpenses));

            var savingsCats = budgets
                .SelectMany(b => b.Categories)
                .Where(c => c.Type == "savings")
                .GroupBy(c => c.Name)
                .Select(g => new BudgetCategory
                {
                    Name = g.Key,
                    Amount = g.Sum(x => x.Amount),
                    Type = "savings",
                    IsRecurring = g.First().IsRecurring,
                    DayOfMonth = g.First().DayOfMonth
                }).ToList();

            SavingsCategories = new ObservableCollection<BudgetCategory>(savingsCats);

            var allExpensesList = allData.Values.SelectMany(m => m.Expenses);
            TotalSavings = allExpensesList
                .Where(e => savingsCats.Any(c => c.Name == e.Category))
                .Sum(e => e.Amount);
        }
    }
}
