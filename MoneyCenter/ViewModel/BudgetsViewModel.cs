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
    public partial class BudgetsViewModel : ObservableObject
    {
        private readonly IFinancialService _financialService;

        public BudgetsViewModel(IFinancialService financialService)
        {
            _financialService = financialService;
            LoadBudgets();
        }

        [ObservableProperty]
        private ObservableCollection<Budget> budgets = new();

        private void LoadBudgets()
        {
            var list = _financialService.GetBudgets();
            Budgets = new ObservableCollection<Budget>(list);
        }
    }

}
