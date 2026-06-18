using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MoneyCenter.Model;
using MoneyCenter.ViewModel.Extensions;
using MoneyCenter.ViewModel.Objects;
using MoneyCenter.Services;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

namespace MoneyCenter.ViewModel
{
    public partial class BudgetsViewModel : ObservableObject
    {
        private readonly IModel _model;
        private readonly IToastService _toastService;
        private readonly IConfirmationService _confirmationService;
        private static readonly string ExpenseType = CategoryKind.Expense.ToStorageValue();
        private static readonly string SavingsType = CategoryKind.Savings.ToStorageValue();
        private string defaultBudgetId = string.Empty;

        public BudgetsViewModel(IModel model, IToastService toastService, IConfirmationService confirmationService)
        {
            _model = model;
            _toastService = toastService;
            _confirmationService = confirmationService;
        }

        [ObservableProperty]
        private ObservableCollection<BudgetWrapper> budgets = new();

        [ObservableProperty]
        private ObservableCollection<MasterCategory> masterCategories = new();

        [ObservableProperty]
        private ObservableCollection<BudgetCategory> masterBudgetCategories = new();

        [ObservableProperty]
        private ObservableCollection<CategoryType> categoryTypes = new();

        public List<string> CategoryTypeKeys
        {
            get
            {
                return CategoryKindMetadata.StorageValues.ToList();
            }
        }

        public async Task LoadDataAsync()
        {
            try
            {
                var schemaBudgets = await _model.GetBudgets();
                var cats = await _model.GetMasterCategories();
                var categoryTypes = await _model.GetCategoryTypes();

                MainThread.BeginInvokeOnMainThread(async () =>
                {
                    CategoryTypes.Clear();
                    foreach (var type in categoryTypes)
                        CategoryTypes.Add(type.ToViewModel());
                    OnPropertyChanged(nameof(CategoryTypeKeys));

                    MasterCategories.Clear();
                    foreach (var c in cats)
                        MasterCategories.Add(c.ToViewModel());

                    Budgets.Clear();
                    MasterBudgetCategories.Clear();
                    foreach (var b in schemaBudgets)
                    {
                        var wrap = new BudgetWrapper(b.ToViewModel(), this);
                        await wrap.LoadCategoriesAsync(_model);
                        Budgets.Add(wrap);
                    }

                    var defaultBudget = Budgets.FirstOrDefault(b => b.Budget.IsDefault) ?? Budgets.FirstOrDefault();
                    defaultBudgetId = defaultBudget?.Budget.Id ?? string.Empty;
                    if (defaultBudget != null)
                    {
                        foreach (var category in MasterCategories.OrderBy(c => c.Name))
                        {
                            var assignment = defaultBudget.Categories.FirstOrDefault(c => c.Id == category.Id);
                            var row = new BudgetCategory
                            {
                                Id = category.Id,
                                Name = category.Name,
                                Type = NormalizeCategoryType(category.Type),
                                Amount = assignment?.Amount ?? 0,
                                IsRecurring = assignment?.IsRecurring ?? false,
                                DayOfMonth = assignment?.DayOfMonth,
                                TypeOptions = CategoryTypeKeys
                            };
                            AttachMasterCategoryCommands(row);
                            MasterBudgetCategories.Add(row);
                        }
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

            var budget = new MoneyCenter.Schema.Budget
            {
                Id = Guid.NewGuid().ToString(),
                Name = newName.Trim(),
                IsDefault = false,
                Description = string.Empty
            };

            await _model.AddBudget(budget);
            await CopyDefaultCategoriesToBudgetAsync(budget.Id);
            ShowToast($"Budget \"{budget.Name}\" created.");
            await LoadDataAsync();
        }

        [RelayCommand]
        private async Task DeleteBudget(BudgetWrapper budget)
        {
            bool confirm = await _confirmationService.ConfirmAsync("Delete Budget", $"Delete \"{budget.Budget.Name}\"? This removes the template and its category assignments.");
            if (confirm)
            {
                await _model.DeleteBudget(budget.Budget.Id);
                ShowToast($"Budget \"{budget.Budget.Name}\" deleted.");
                await LoadDataAsync();
            }
        }

        [RelayCommand]
        private async Task AddCategory()
        {
            if (MasterBudgetCategories.Any(c => c.Id == 0))
                return;

            var category = new BudgetCategory
            {
                Type = ExpenseType,
                TypeOptions = CategoryTypeKeys,
                IsEditing = true
            };
            AttachMasterCategoryCommands(category);
            MasterBudgetCategories.Insert(0, category);
        }

        [RelayCommand]
        private async Task DeleteCategory(MasterCategory cat)
        {
            bool confirm = await _confirmationService.ConfirmAsync("Delete Category", $"Delete \"{cat.Name}\"? This removes it from every budget template.");
            if (confirm)
            {
                await _model.DeleteMasterCategory(cat.Id);
                ShowToast($"Category '{cat.Name}' deleted.");
                await LoadDataAsync();
            }
        }

        [RelayCommand]
        private async Task DeleteBudgetCategory(BudgetCategory cat)
        {
            var master = MasterCategories.FirstOrDefault(c => c.Id == cat.Id);
            if (master != null)
                await DeleteCategory(master);
        }

        private void AttachMasterCategoryCommands(BudgetCategory category)
        {
            category.EditCommand = new RelayCommand(() => category.IsEditing = true);
            category.CancelCommand = new AsyncRelayCommand(async () => await LoadDataAsync());
            category.SaveCommand = new AsyncRelayCommand(async () => await SaveMasterCategoryAsync(category));
            category.DeleteCommand = new AsyncRelayCommand(async () => await DeleteBudgetCategory(category));
        }

        private async Task SaveMasterCategoryAsync(BudgetCategory category)
        {
            var name = category.Name?.Trim() ?? string.Empty;
            if (string.IsNullOrWhiteSpace(name))
            {
                ShowToast("Category name is required.");
                return;
            }

            var duplicate = MasterCategories.Any(c => c.Id != category.Id && c.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
            if (duplicate)
            {
                ShowToast("A category with this name already exists.");
                return;
            }

            if (!HasPositiveAmount(category.Amount))
            {
                ShowToast("Default amount is required.");
                return;
            }

            category.Type = NormalizeCategoryType(category.Type);
            if (!ValidateRecurringDay(category.IsRecurring, category.DayOfMonth))
            {
                ShowToast("Recurring day is required and must be between 1 and 31.");
                return;
            }

            if (category.Id == 0)
            {
                await _model.AddMasterCategory(new MoneyCenter.Schema.MasterCategory
                {
                    Name = name,
                    Type = category.Type
                });

                var created = (await _model.GetMasterCategories()).FirstOrDefault(c => c.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
                if (created != null)
                {
                    category.Id = created.Id;
                }
            }
            else
            {
                var existing = await _model.GetMasterCategoryById(category.Id);
                await _model.UpdateMasterCategory(new MoneyCenter.Schema.MasterCategory
                {
                    Id = category.Id,
                    Name = name,
                    Type = category.Type,
                    Description = existing?.Description ?? string.Empty
                });
            }

            if (!string.IsNullOrWhiteSpace(defaultBudgetId) && category.Id != 0)
            {
                var assignments = await _model.GetCategoriesByBudgetId(defaultBudgetId);
                var exists = assignments.Any(a => a.MasterCategoryId == category.Id);
                if (exists)
                {
                    await _model.UpdateBudgetCategoryAssignment(defaultBudgetId, category.Id, category.Amount!.Value, category.IsRecurring, category.IsRecurring ? category.DayOfMonth ?? 1 : null);
                }
                else
                {
                    await _model.AssignCategoryToBudget(defaultBudgetId, category.Id, category.Amount!.Value, category.IsRecurring, category.IsRecurring ? category.DayOfMonth ?? 1 : null);
                }
            }

            if (category.Type == SavingsType && category.Id != 0)
            {
                await _model.CreateSavingsPod(category.Id);
            }

            ShowToast($"Category \"{name}\" has been updated.");
            await LoadDataAsync();
        }

        internal async Task SaveBudgetAsync(BudgetWrapper budget)
        {
            var name = budget.Budget.Name?.Trim() ?? string.Empty;
            if (string.IsNullOrWhiteSpace(name))
            {
                ShowToast("Budget name is required.");
                return;
            }

            budget.Budget.Name = name;
            await _model.UpdateBudget(new MoneyCenter.Schema.Budget
            {
                Id = budget.Budget.Id,
                Name = budget.Budget.Name,
                IsDefault = budget.Budget.IsDefault,
                Description = budget.Budget.Description
            });

            foreach (var category in budget.Categories)
            {
                if (!HasPositiveAmount(category.Amount))
                {
                    ShowToast($"Amount is required for \"{category.Name}\".");
                    return;
                }

                if (!ValidateRecurringDay(category.IsRecurring, category.DayOfMonth))
                {
                    ShowToast($"Recurring day is required for \"{category.Name}\".");
                    return;
                }

                await _model.UpdateBudgetCategoryAssignment(
                    budget.Budget.Id,
                    category.Id,
                    category.Amount!.Value,
                    category.IsRecurring,
                    category.IsRecurring ? category.DayOfMonth ?? 1 : null);
            }

            ShowToast($"Budget \"{budget.Budget.Name}\" has been updated.");
            await LoadDataAsync();
        }

        internal async Task CancelBudgetEditAsync(BudgetWrapper budget)
        {
            budget.IsEditing = false;
            budget.IsAddingCategory = false;
            budget.SelectedCategory = null;
            budget.AvailableCategories.Clear();
            await budget.LoadCategoriesAsync(_model);
        }

        internal async Task RemoveCategoryFromBudgetAsync(BudgetWrapper budget, BudgetCategory category)
        {
            await _model.RemoveCategoryFromBudget(budget.Budget.Id, category.Id);
            budget.Categories.Remove(category);
            ShowToast($"Category \"{category.Name}\" removed from \"{budget.Budget.Name}\".");
        }

        internal async Task BeginAddCategoryToBudgetAsync(BudgetWrapper budget)
        {
            var assignedIds = budget.Categories.Select(c => c.Id).ToHashSet();
            var available = MasterCategories.Where(c => !assignedIds.Contains(c.Id)).OrderBy(c => c.Name).ToList();
            if (!available.Any())
            {
                ShowToast("All categories are already in this budget.");
                return;
            }

            budget.AvailableCategories.Clear();
            foreach (var category in available)
            {
                budget.AvailableCategories.Add(category);
            }

            budget.SelectedCategory = budget.AvailableCategories.FirstOrDefault();
            budget.NewCategoryAmount = 0;
            budget.NewCategoryIsRecurring = false;
            budget.NewCategoryDayOfMonth = null;
            budget.IsAddingCategory = true;
        }

        internal async Task ConfirmAddCategoryToBudgetAsync(BudgetWrapper budget)
        {
            var category = budget.SelectedCategory;
            if (category == null)
            {
                ShowToast("Select a category to add.");
                return;
            }

            if (budget.Categories.Any(c => c.Id == category.Id))
            {
                ShowToast($"Category \"{category.Name}\" is already in this budget.");
                budget.IsAddingCategory = false;
                return;
            }

            if (!HasPositiveAmount(budget.NewCategoryAmount))
            {
                ShowToast("Amount is required.");
                return;
            }

            if (!ValidateRecurringDay(budget.NewCategoryIsRecurring, budget.NewCategoryDayOfMonth))
            {
                ShowToast("Recurring day is required and must be between 1 and 31.");
                return;
            }

            await _model.AssignCategoryToBudget(
                budget.Budget.Id,
                category.Id,
                budget.NewCategoryAmount!.Value,
                budget.NewCategoryIsRecurring,
                budget.NewCategoryIsRecurring ? budget.NewCategoryDayOfMonth ?? 1 : null);

            budget.IsAddingCategory = false;
            budget.SelectedCategory = null;
            budget.AvailableCategories.Clear();
            await budget.LoadCategoriesAsync(_model);
            ShowToast($"Category \"{category.Name}\" added to \"{budget.Budget.Name}\".");
        }

        private static string NormalizeCategoryType(string? type)
        {
            return CategoryKindExtensions.FromStorageValue(type).ToStorageValue();
        }

        private static bool ValidateRecurringDay(bool isRecurring, int? dayOfMonth)
        {
            return !isRecurring || (dayOfMonth.HasValue && dayOfMonth >= 1 && dayOfMonth <= 31);
        }

        private static bool HasPositiveAmount(decimal? amount)
        {
            return amount.HasValue && amount.Value > 0;
        }

        private void ShowToast(string message)
        {
            _toastService.Show(message);
        }

        private async Task CopyDefaultCategoriesToBudgetAsync(string budgetId)
        {
            if (string.IsNullOrWhiteSpace(defaultBudgetId))
            {
                return;
            }

            var assignments = await _model.GetCategoriesByBudgetId(defaultBudgetId);
            foreach (var assignment in assignments)
            {
                await _model.AssignCategoryToBudget(
                    budgetId,
                    assignment.MasterCategoryId,
                    assignment.Amount,
                    assignment.IsRecurring,
                    assignment.DayOfMonth);
            }
        }

    }

    public partial class BudgetWrapper : ObservableObject
    {
        private readonly BudgetsViewModel _parent;
        public Budget Budget { get; }
        
        [ObservableProperty]
        private ObservableCollection<BudgetCategory> categories = new();

        [ObservableProperty]
        private ObservableCollection<MasterCategory> availableCategories = new();

        [ObservableProperty]
        private MasterCategory? selectedCategory;

        [ObservableProperty]
        private decimal? newCategoryAmount;

        [ObservableProperty]
        private bool newCategoryIsRecurring;

        [ObservableProperty]
        private int? newCategoryDayOfMonth;

        [ObservableProperty]
        private bool isAddingCategory;

        [ObservableProperty]
        private bool isEditing;
        
        public BudgetWrapper(Budget budget, BudgetsViewModel parent)
        {
            Budget = budget;
            _parent = parent;
        }

        public async Task LoadCategoriesAsync(IModel model)
        {
            var assignments = await model.GetCategoriesByBudgetId(Budget.Id);
            var masterCategories = await model.GetMasterCategories();
            Categories.Clear();
            foreach (var a in assignments)
            {
                var master = masterCategories.FirstOrDefault(c => c.Id == a.MasterCategoryId);
                var category = new BudgetCategory
                {
                    Id = a.MasterCategoryId,
                    Name = master?.Name ?? $"Category {a.MasterCategoryId}",
                    Type = master?.Type ?? string.Empty,
                    Amount = a.Amount,
                    IsRecurring = a.IsRecurring,
                    DayOfMonth = a.DayOfMonth,
                    IsEditing = IsEditing
                };
                category.DeleteCommand = new AsyncRelayCommand(async () => await RemoveCategory(category));
                Categories.Add(category);
            }
        }

        [RelayCommand]
        private void Edit()
        {
            IsEditing = true;
        }

        partial void OnIsEditingChanged(bool value)
        {
            foreach (var category in Categories)
            {
                category.IsEditing = value;
            }
        }

        [RelayCommand]
        private async Task Save()
        {
            await _parent.SaveBudgetAsync(this);
        }

        [RelayCommand]
        private async Task Cancel()
        {
            await _parent.CancelBudgetEditAsync(this);
        }

        [RelayCommand]
        private async Task RemoveCategory(BudgetCategory category)
        {
            await _parent.RemoveCategoryFromBudgetAsync(this, category);
        }

        [RelayCommand]
        private async Task AddCategory()
        {
            await _parent.BeginAddCategoryToBudgetAsync(this);
        }

        [RelayCommand]
        private async Task ConfirmAddCategory()
        {
            await _parent.ConfirmAddCategoryToBudgetAsync(this);
        }

        [RelayCommand]
        private void CancelAddCategory()
        {
            IsAddingCategory = false;
            SelectedCategory = null;
            AvailableCategories.Clear();
        }

        partial void OnNewCategoryIsRecurringChanged(bool value)
        {
            if (!value)
            {
                NewCategoryDayOfMonth = null;
            }
        }
    }
}
