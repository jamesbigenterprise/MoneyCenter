using CommunityToolkit.Mvvm.ComponentModel;
using MoneyCenter.Services;
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
        private readonly IFinancialService _financialService;

        public SavingsViewModel(IFinancialService financialService)
        {
            _financialService = financialService;
            LoadSavings();
        }

        [ObservableProperty]
        private decimal totalSavings;

        [ObservableProperty]
        private ObservableCollection<BudgetCategory> savingsCategories = new();

        private void LoadSavings()
        {
            var budgets = _financialService.GetBudgets();
            var allData = _financialService.GetAllData();

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

            var allExpenses = allData.Values.SelectMany(m => m.Expenses);
            TotalSavings = allExpenses
                .Where(e => savingsCats.Any(c => c.Name == e.Category))
                .Sum(e => e.Amount);
        }
    }

}
