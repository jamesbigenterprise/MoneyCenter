using CommunityToolkit.Mvvm.ComponentModel;
using MoneyCenter.Model;
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
    public partial class BudgetsViewModel : ObservableObject
    {
        private readonly IModel _model;

        public BudgetsViewModel(IModel model)
        {
            _model = model;
            LoadBudgets();
        }

        [ObservableProperty]
        private ObservableCollection<Budget> budgets = new();

        [ObservableProperty]
        private ObservableCollection<BudgetMasterCategory> categoryAssignments = new();

        private async void LoadBudgets()
        {
            try
            {
                var schemaBudgets = await _model.GetBudgets();
                
                var viewModelBudgets = schemaBudgets.Select(b => new Budget
                {
                    Id = b.Id,
                    Name = b.Name,
                    Description = b.Description,
                    IsDefault = b.IsDefault
                }).ToList();
                
                Budgets = new ObservableCollection<Budget>(viewModelBudgets);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"LoadBudgets error: {ex.Message}");
            }
        }

        public async Task LoadCategoriesForBudget(string budgetId)
        {
            try
            {
                var assignments = await _model.GetCategoriesByBudgetId(budgetId);
                CategoryAssignments = new ObservableCollection<BudgetMasterCategory>(assignments.Select(a => new BudgetMasterCategory
                {
                    Id = a.Id,
                    BudgetId = a.BudgetId,
                    MasterCategoryId = a.MasterCategoryId,
                    Amount = a.Amount,
                    IsRecurring = a.IsRecurring,
                    DayOfMonth = a.DayOfMonth
                }).ToList());
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"LoadCategoriesForBudget error: {ex.Message}");
            }
        }
    }
}
