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
            ///TODO
        }
    }
}
