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

        private async void LoadBudgets()
        {
            var schemaBudgets = await _model.GetBudgets();
            var allCategories = await _model.GetMasterCategories();
            
            var viewModelBudgets = schemaBudgets
                .Select(b => b.ToViewModel(allCategories))
                .ToList();
            
            Budgets = new ObservableCollection<Budget>(viewModelBudgets);
        }
    }
}
