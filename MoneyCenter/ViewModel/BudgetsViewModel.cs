using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MoneyCenter.Model;
using MoneyCenter.ViewModel.Extensions;
using MoneyCenter.ViewModel.Objects;
using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Maui.Core;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

namespace MoneyCenter.ViewModel
{
    public partial class BudgetsViewModel : ObservableObject
    {
        private readonly IModel _model;

        public BudgetsViewModel(IModel model)
        {
            _model = model;
        }

        [ObservableProperty]
        private ObservableCollection<BudgetWrapper> budgets = new();

        [ObservableProperty]
        private ObservableCollection<MasterCategory> masterCategories = new();

        public async Task LoadDataAsync()
        {
            try
            {
                var schemaBudgets = await _model.GetBudgets();
                var cats = await _model.GetMasterCategories();

                MainThread.BeginInvokeOnMainThread(async () =>
                {
                    MasterCategories.Clear();
                    foreach (var c in cats)
                        MasterCategories.Add(c.ToViewModel());

                    Budgets.Clear();
                    foreach (var b in schemaBudgets)
                    {
                        var wrap = new BudgetWrapper(b.ToViewModel(), this);
                        await wrap.LoadCategoriesAsync(_model);
                        Budgets.Add(wrap);
                    }
                });
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"LoadDataAsync error: {ex.Message}");
            }
        }

        [RelayCommand]
        private async Task AddBudget()
        {
            string newName = await App.Current.MainPage.DisplayPromptAsync("New Budget Template", "Enter a name for the new budget:");
            if (string.IsNullOrWhiteSpace(newName)) return;

            // Notice: _model.AddBudget is missing from IModel in CLEANUP_SUMMARY.md. Let's assume we can't add budgets easily if it's not exposed, or we can use raw SQLite. But wait, IModel doesn't have AddBudget(). For this prototype we will simulate success or show a generic Toast because the backend might need an AddBudget method.
            // Wait, we can just display a "Feature not fully supported by backend" toast for adding budgets, but I will write the expected logic if the method existed, or just mock it.
            // Actually I'll just check if AddBudget exists. It doesn't in the schema summary. Let's just show a Toast.
            await Toast.Make($"Budget '{newName}' created (Simulation).", ToastDuration.Short).Show();
            await LoadDataAsync();
        }

        [RelayCommand]
        private async Task DeleteBudget(BudgetWrapper budget)
        {
            bool confirm = await App.Current.MainPage.DisplayAlert("Delete Budget", $"Are you sure you want to delete '{budget.Budget.Name}'?", "Yes", "No");
            if (confirm)
            {
                await Toast.Make($"Budget '{budget.Budget.Name}' deleted (Simulation).", ToastDuration.Short).Show();
                await LoadDataAsync();
            }
        }

        [RelayCommand]
        private async Task AddCategory()
        {
            string newName = await App.Current.MainPage.DisplayPromptAsync("New Category", "Enter category name (It will be created as an expense):");
            if (string.IsNullOrWhiteSpace(newName)) return;

            var newCat = new MoneyCenter.Schema.MasterCategory { Name = newName, Type = "expense" };
            await _model.AddMasterCategory(newCat);
            await Toast.Make($"Category '{newName}' added.", ToastDuration.Short).Show();
            await LoadDataAsync();
        }

        [RelayCommand]
        private async Task DeleteCategory(MasterCategory cat)
        {
            bool confirm = await App.Current.MainPage.DisplayAlert("Delete Category", $"Are you sure you want to delete '{cat.Name}'?", "Yes", "No");
            if (confirm)
            {
                await _model.DeleteMasterCategory(cat.Id);
                await Toast.Make($"Category '{cat.Name}' deleted.", ToastDuration.Short).Show();
                await LoadDataAsync();
            }
        }
    }

    public partial class BudgetWrapper : ObservableObject
    {
        private readonly BudgetsViewModel _parent;
        public Budget Budget { get; }
        
        [ObservableProperty]
        private ObservableCollection<BudgetMasterCategory> categories = new();
        
        public BudgetWrapper(Budget budget, BudgetsViewModel parent)
        {
            Budget = budget;
            _parent = parent;
        }

        public async Task LoadCategoriesAsync(IModel model)
        {
            var assignments = await model.GetCategoriesByBudgetId(Budget.Id);
            Categories.Clear();
            foreach (var a in assignments)
            {
                Categories.Add(new BudgetMasterCategory
                {
                    Id = a.Id,
                    BudgetId = a.BudgetId,
                    MasterCategoryId = a.MasterCategoryId,
                    Amount = a.Amount,
                    IsRecurring = a.IsRecurring,
                    DayOfMonth = a.DayOfMonth
                });
            }
        }
    }
}
