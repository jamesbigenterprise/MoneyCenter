using CommunityToolkit.Mvvm.ComponentModel;
using MoneyCenter.Model;
using CommunityToolkit.Mvvm.Input;
using MoneyCenter.Services;
using MoneyCenter.ViewModel.Extensions;
using MoneyCenter.ViewModel.Objects;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Schema = MoneyCenter.Schema;

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

        [ObservableProperty]
        private ObservableCollection<BudgetCategory> visibleSavingsCategories = new();

        [ObservableProperty]
        private ObservableCollection<BudgetCategory> hiddenSavingsCategories = new();

        [ObservableProperty]
        private bool isAddingBucket;

        [ObservableProperty]
        private string newBucketName = string.Empty;

        [ObservableProperty]
        private decimal newBucketAmount;

        private async void LoadSavings()
        {
            try
            {
                var categories = await _model.GetMasterCategoriesByType("savings");
                var budgets = await _model.GetBudgets();
                var defaultBudget = budgets.FirstOrDefault(b => b.IsDefault) ?? budgets.FirstOrDefault();
                var assignments = defaultBudget is null
                    ? new List<Schema.BudgetMasterCategory>()
                    : await _model.GetCategoriesByBudgetId(defaultBudget.Id);
                var buckets = new List<BudgetCategory>();

                foreach (var category in categories)
                {
                    var assignment = assignments.FirstOrDefault(a => a.MasterCategoryId == category.Id);
                    var pod = await _model.GetSavingsPodByMasterCategoryId(category.Id);
                    buckets.Add(new BudgetCategory
                    {
                        Id = category.Id,
                        Name = category.Name,
                        Type = category.Type,
                        Amount = assignment?.Amount ?? 0,
                        CurrentBalance = pod?.CurrentBalance ?? 0,
                        IsRecurring = assignment?.IsRecurring ?? false,
                        DayOfMonth = assignment?.DayOfMonth
                    });
                }

                MainThread.BeginInvokeOnMainThread(() =>
                {
                    SavingsCategories.Clear();
                    VisibleSavingsCategories.Clear();
                    HiddenSavingsCategories.Clear();

                    foreach (var bucket in buckets)
                    {
                        SavingsCategories.Add(bucket);
                        VisibleSavingsCategories.Add(bucket);
                    }

                    TotalSavings = SavingsCategories.Sum(c => c.CurrentBalance);
                });
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"LoadSavings error: {ex.Message}");
            }
        }

        [RelayCommand]
        private void StartAddBucket()
        {
            NewBucketName = string.Empty;
            NewBucketAmount = 0;
            IsAddingBucket = true;
        }

        [RelayCommand]
        private void CancelAddBucket()
        {
            IsAddingBucket = false;
            NewBucketName = string.Empty;
            NewBucketAmount = 0;
        }

        [RelayCommand]
        private async Task SaveBucket()
        {
            if (string.IsNullOrWhiteSpace(NewBucketName))
                return;

            var category = new Schema.MasterCategory
            {
                Name = NewBucketName.Trim(),
                Type = "savings",
                Description = $"{NewBucketName.Trim()} savings bucket"
            };

            await _model.AddMasterCategory(category);

            if (category.Id > 0)
            {
                await _model.CreateSavingsPod(category.Id);

                var budgets = await _model.GetBudgets();
                var defaultBudget = budgets.FirstOrDefault(b => b.IsDefault) ?? budgets.FirstOrDefault();
                if (defaultBudget is not null)
                    await _model.AssignCategoryToBudget(defaultBudget.Id, category.Id, NewBucketAmount, true, 1);
            }

            IsAddingBucket = false;
            NewBucketName = string.Empty;
            NewBucketAmount = 0;
            LoadSavings();
        }

        [RelayCommand]
        private void HideBucket(BudgetCategory category)
        {
            if (category is null)
                return;

            category.IsHidden = true;
            VisibleSavingsCategories.Remove(category);
            if (!HiddenSavingsCategories.Contains(category))
                HiddenSavingsCategories.Add(category);
        }

        [RelayCommand]
        private void ShowBucket(BudgetCategory category)
        {
            if (category is null)
                return;

            category.IsHidden = false;
            HiddenSavingsCategories.Remove(category);
            if (!VisibleSavingsCategories.Contains(category))
                VisibleSavingsCategories.Add(category);
        }
    }
}
