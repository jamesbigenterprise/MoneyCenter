using MoneyCenter.ViewModel.Objects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace MoneyCenter.Services
{
    public class FinancialService : IFinancialService
    {
        private Dictionary<string, MonthlyData> _data;
        private List<Budget> _budgets;
        private List<BudgetCategory> _masterCategories;
        private readonly string _dataFile;
        private readonly string _budgetFile;
        private readonly string _categoryFile;

        public FinancialService()
        {
            var appDataDir = FileSystem.AppDataDirectory;
            _dataFile = Path.Combine(appDataDir, "financial_data.json");
            _budgetFile = Path.Combine(appDataDir, "budgets.json");
            _categoryFile = Path.Combine(appDataDir, "categories.json");

            // Initialize with data
            _data = LoadData();
            _budgets = LoadBudgets();
            _masterCategories = LoadCategories();

            // If there's no data yet, create initial data
            if (_data.Count == 0)
            {
                _data = InitializeYearData(DateTime.Now.Year);
            }

            if (_budgets.Count == 0)
            {
                _budgets = GetInitialBudgets();
                SaveBudgets();
            }

            if (_masterCategories.Count == 0)
            {
                _masterCategories = ExtractCategoriesFromBudgets(_budgets);
                SaveCategories();
            }
        }

        public Dictionary<string, MonthlyData> GetAllData() => _data;

        public List<Budget> GetBudgets() => _budgets;

        public List<string> GetAllCategories() => _masterCategories.Select(c => c.Name).ToList();

        public List<BudgetCategory> GetMasterCategories() => _masterCategories;

        public async Task AddCategoryAsync(BudgetCategory category)
        {
            if (_masterCategories.Any(c => c.Name.Equals(category.Name, StringComparison.OrdinalIgnoreCase)))
                return;

            _masterCategories.Add(category);
            await SaveCategoriesAsync();
        }

        public async Task AddExpense(string month, Expense expense)
        {
            if (!_data.ContainsKey(month))
            {
                var defaultBudget = _budgets.FirstOrDefault(b => b.IsDefault);
                _data[month] = new MonthlyData
                {
                    Income = defaultBudget?.Categories.FirstOrDefault(c => c.Type == "income")?.Amount ?? 0,
                    Expenses = new List<Expense>(),
                    BudgetId = defaultBudget?.Id
                };
            }

            _data[month].Expenses.Add(expense);
            await SaveDataAsync();
        }

        public async Task UpdateExpense(string month, Expense expense)
        {
            if (!_data.ContainsKey(month)) return;

            var existingExpense = _data[month].Expenses.FirstOrDefault(e => e.Id == expense.Id);
            if (existingExpense != null)
            {
                var index = _data[month].Expenses.IndexOf(existingExpense);
                _data[month].Expenses[index] = expense;
                await SaveDataAsync();
            }
        }

        public async Task DeleteExpense(string month, string expenseId)
        {
            if (!_data.ContainsKey(month)) return;

            var existingExpense = _data[month].Expenses.FirstOrDefault(e => e.Id == expenseId);
            if (existingExpense != null)
            {
                _data[month].Expenses.Remove(existingExpense);
                await SaveDataAsync();
            }
        }

        public void SetBudgetForMonth(string budgetId, string month)
        {
            if (!_data.ContainsKey(month))
            {
                var defaultBudget = _budgets.FirstOrDefault(b => b.IsDefault);
                _data[month] = new MonthlyData
                {
                    Income = defaultBudget?.Categories.FirstOrDefault(c => c.Type == "income")?.Amount ?? 0,
                    Expenses = new List<Expense>(),
                    BudgetId = budgetId
                };
            }
            else
            {
                _data[month].BudgetId = budgetId;
            }

            SaveData();
        }

        public void SetIncomeForMonth(decimal income, string month)
        {
            if (!_data.ContainsKey(month))
            {
                var defaultBudget = _budgets.FirstOrDefault(b => b.IsDefault);
                _data[month] = new MonthlyData
                {
                    Income = income,
                    Expenses = new List<Expense>(),
                    BudgetId = defaultBudget?.Id
                };
            }
            else
            {
                _data[month].Income = income;
            }

            SaveData();
        }

        public async Task<bool> AddNewMonth()
        {
            var lastMonth = _data.Keys.OrderBy(k => k).LastOrDefault();
            if (string.IsNullOrEmpty(lastMonth)) return false;

            var lastDate = DateTime.Parse($"{lastMonth}-01");
            var newDate = lastDate.AddMonths(1);
            var newMonth = newDate.ToString("yyyy-MM");

            if (_data.ContainsKey(newMonth)) return false;

            var defaultBudget = _budgets.FirstOrDefault(b => b.IsDefault);
            var defaultIncome = defaultBudget?.Categories.FirstOrDefault(c => c.Type == "income")?.Amount ?? 0;

            _data[newMonth] = new MonthlyData
            {
                Income = defaultIncome,
                Expenses = new List<Expense>(),
                BudgetId = defaultBudget?.Id
            };

            await SaveDataAsync();
            return true;
        }

        public Dictionary<string, MonthlyData> InitializeYearData(int year)
        {
            var yearData = new Dictionary<string, MonthlyData>();
            var defaultBudget = _budgets.FirstOrDefault(b => b.IsDefault);
            var defaultIncome = defaultBudget?.Categories.FirstOrDefault(c => c.Type == "income")?.Amount ?? 5000;

            for (int i = 0; i < 12; i++)
            {
                var date = new DateTime(year, 1, 1).AddMonths(i);
                var monthKey = date.ToString("yyyy-MM");
                yearData[monthKey] = new MonthlyData
                {
                    Income = defaultIncome,
                    Expenses = new List<Expense>(),
                    BudgetId = defaultBudget?.Id
                };
            }

            return yearData;
        }

        public List<Budget> GetInitialBudgets()
        {
            return new List<Budget>
        {
            new Budget
            {
                Id = Guid.NewGuid().ToString(),
                Name = "Basic Budget",
                IsDefault = true,
                Description = "A simple budget for getting started",
                Categories = new List<BudgetCategory>
                {
                    new BudgetCategory { Name = "Income", Type = "income", Amount = 5000, IsRecurring = true, DayOfMonth = 1 },
                    new BudgetCategory { Name = "Housing", Type = "expense", Amount = 1500, IsRecurring = true, DayOfMonth = 1 },
                    new BudgetCategory { Name = "Food", Type = "expense", Amount = 500, IsRecurring = false },
                    new BudgetCategory { Name = "Utilities", Type = "expense", Amount = 300, IsRecurring = true, DayOfMonth = 15 },
                    new BudgetCategory { Name = "Transportation", Type = "expense", Amount = 200, IsRecurring = false },
                    new BudgetCategory { Name = "Entertainment", Type = "expense", Amount = 200, IsRecurring = false },
                    new BudgetCategory { Name = "Savings", Type = "savings", Amount = 500, IsRecurring = true, DayOfMonth = 1 }
                }
            },
            new Budget
            {
                Id = Guid.NewGuid().ToString(),
                Name = "Frugal Budget",
                IsDefault = false,
                Description = "A budget focused on maximizing savings",
                Categories = new List<BudgetCategory>
                {
                    new BudgetCategory { Name = "Income", Type = "income", Amount = 5000, IsRecurring = true, DayOfMonth = 1 },
                    new BudgetCategory { Name = "Housing", Type = "expense", Amount = 1200, IsRecurring = true, DayOfMonth = 1 },
                    new BudgetCategory { Name = "Food", Type = "expense", Amount = 300, IsRecurring = false },
                    new BudgetCategory { Name = "Utilities", Type = "expense", Amount = 250, IsRecurring = true, DayOfMonth = 15 },
                    new BudgetCategory { Name = "Transportation", Type = "expense", Amount = 150, IsRecurring = false },
                    new BudgetCategory { Name = "Entertainment", Type = "expense", Amount = 100, IsRecurring = false },
                    new BudgetCategory { Name = "Savings", Type = "savings", Amount = 1000, IsRecurring = true, DayOfMonth = 1 }
                }
            }
        };
        }

        private List<BudgetCategory> ExtractCategoriesFromBudgets(List<Budget> budgets)
        {
            var categories = new Dictionary<string, BudgetCategory>(StringComparer.OrdinalIgnoreCase);

            foreach (var budget in budgets)
            {
                foreach (var category in budget.Categories)
                {
                    if (!categories.ContainsKey(category.Name))
                    {
                        categories[category.Name] = category;
                    }
                }
            }

            return categories.Values.ToList();
        }

        private Dictionary<string, MonthlyData> LoadData()
        {
            if (!File.Exists(_dataFile))
                return new Dictionary<string, MonthlyData>();

            var json = File.ReadAllText(_dataFile);
            try
            {
                return JsonSerializer.Deserialize<Dictionary<string, MonthlyData>>(json) ?? new Dictionary<string, MonthlyData>();
            }
            catch
            {
                return new Dictionary<string, MonthlyData>();
            }
        }

        private List<Budget> LoadBudgets()
        {
            if (!File.Exists(_budgetFile))
                return new List<Budget>();

            var json = File.ReadAllText(_budgetFile);
            try
            {
                return JsonSerializer.Deserialize<List<Budget>>(json) ?? new List<Budget>();
            }
            catch
            {
                return new List<Budget>();
            }
        }

        private List<BudgetCategory> LoadCategories()
        {
            if (!File.Exists(_categoryFile))
                return new List<BudgetCategory>();

            var json = File.ReadAllText(_categoryFile);
            try
            {
                return JsonSerializer.Deserialize<List<BudgetCategory>>(json) ?? new List<BudgetCategory>();
            }
            catch
            {
                return new List<BudgetCategory>();
            }
        }

        private void SaveData()
        {
            var json = JsonSerializer.Serialize(_data);
            File.WriteAllText(_dataFile, json);
        }

        private async Task SaveDataAsync()
        {
            var json = JsonSerializer.Serialize(_data);
            await File.WriteAllTextAsync(_dataFile, json);
        }

        private void SaveBudgets()
        {
            var json = JsonSerializer.Serialize(_budgets);
            File.WriteAllText(_budgetFile, json);
        }

        private async Task SaveBudgetsAsync()
        {
            var json = JsonSerializer.Serialize(_budgets);
            await File.WriteAllTextAsync(_budgetFile, json);
        }

        private void SaveCategories()
        {
            var json = JsonSerializer.Serialize(_masterCategories);
            File.WriteAllText(_categoryFile, json);
        }

        private async Task SaveCategoriesAsync()
        {
            var json = JsonSerializer.Serialize(_masterCategories);
            await File.WriteAllTextAsync(_categoryFile, json);
        }
    }

}
