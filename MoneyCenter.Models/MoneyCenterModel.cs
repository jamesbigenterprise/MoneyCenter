using MoneyCenter.Schema;
using MoneyCenter.SQLiteWrapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MoneyCenter.Model
{
    public class MoneyCenterModel : IModel
    {
        private readonly MoneyCenterDatabase _database;

        public MoneyCenterModel(MoneyCenterDatabase database)
        {
            if (database == null)
            {
                //TODO have the permissions page handle this instead of throwing an exception

                throw new ArgumentNullException(nameof(database));
            }

            _database = database;
        }

        public async Task InitializeDatabase()
        {
            await _database.InitializeAsync();
        }

        //public async Task<List<SingleEntryDataModel>> GetAllEntries()
        //{
        //    await InitializeDatabase();
        //    return await _database.GetAllEntries();
        //}


        public async Task<Dictionary<string, Schema.MonthlyData>> GetAllData()
        {
            await InitializeDatabase();
            var monthlyDataList = await _database.GetAllMonthlyDataAsync();
            
            return monthlyDataList.ToDictionary(m => m.Month, m => m);
        }

        public async Task<List<Schema.Budget>> GetBudgets()
        {
            await InitializeDatabase();
            return await _database.GetAllBudgetsAsync();
        }

        public async Task<List<string>> GetAllCategories()
        {
            await InitializeDatabase();
            var categories = await _database.GetAllCategoriesAsync();
            return categories.Select(c => c.Name).Distinct().ToList();
        }

        public async Task<List<Schema.BudgetCategory>> GetMasterCategories()
        {
            await InitializeDatabase();
            return await _database.GetAllCategoriesAsync();
        }

        public async Task<List<Schema.BudgetCategory>> GetCategoriesByBudgetId(string budgetId)
        {
            await InitializeDatabase();
            return await _database.GetCategoriesByBudgetIdAsync(budgetId);
        }

        public async Task<List<Schema.Expense>> GetExpensesByMonth(string month)
        {
            await InitializeDatabase();
            return await _database.GetExpensesByMonthAsync(month);
        }

        public async Task<List<Schema.Expense>> GetAllExpenses()
        {
            await InitializeDatabase();
            return await _database.GetAllExpensesAsync();
        }

        public async Task AddCategoryAsync(Schema.BudgetCategory category)
        {
            await InitializeDatabase();
            var existing = await _database.GetCategoryByNameAsync(category.Name);
            if (existing != null) return;

            await _database.InsertCategoryAsync(category);
        }

        public async Task AddExpense(string month, Schema.Expense expense)
        {
            await InitializeDatabase();
            
            // Ensure the month exists
            var monthlyData = await _database.GetMonthlyDataAsync(month);
            if (monthlyData == null)
            {
                var defaultBudget = (await _database.GetAllBudgetsAsync())
                    .FirstOrDefault(b => b.IsDefault);
                
                monthlyData = new Schema.MonthlyData
                {
                    Month = month,
                    Income = 0,
                    BudgetId = defaultBudget?.Id
                };
                await _database.InsertMonthlyDataAsync(monthlyData);
            }

            // Set the MonthId and insert the expense
            expense.MonthId = month;
            await _database.InsertExpenseAsync(expense);
        }

        public async Task UpdateExpense(string month, Schema.Expense expense)
        {
            await InitializeDatabase();
            
            var existingExpense = await _database.GetExpenseAsync(expense.Id);
            if (existingExpense != null)
            {
                expense.MonthId = month;
                await _database.UpdateExpenseAsync(expense);
            }
        }

        public async Task DeleteExpense(string month, string expenseId)
        {
            await InitializeDatabase();
            await _database.DeleteExpenseByIdAsync(expenseId);
        }

        public async Task SetBudgetForMonth(string budgetId, string month)
        {
            await InitializeDatabase();
            
            var monthlyData = await _database.GetMonthlyDataAsync(month);
            if (monthlyData == null)
            {
                monthlyData = new Schema.MonthlyData
                {
                    Month = month,
                    Income = 0,
                    BudgetId = budgetId
                };
                await _database.InsertMonthlyDataAsync(monthlyData);
            }
            else
            {
                monthlyData.BudgetId = budgetId;
                await _database.UpdateMonthlyDataAsync(monthlyData);
            }
        }

        public async Task SetIncomeForMonth(decimal income, string month)
        {
            await InitializeDatabase();
            
            var monthlyData = await _database.GetMonthlyDataAsync(month);
            if (monthlyData == null)
            {
                monthlyData = new Schema.MonthlyData
                {
                    Month = month,
                    Income = income,
                    BudgetId = null
                };
                await _database.InsertMonthlyDataAsync(monthlyData);
            }
            else
            {
                monthlyData.Income = income;
                await _database.UpdateMonthlyDataAsync(monthlyData);
            }
        }

        public async Task<bool> AddNewMonth()
        {
            await InitializeDatabase();
            
            var allMonths = await _database.GetAllMonthlyDataAsync();
            var lastMonth = allMonths.OrderBy(m => m.Month).LastOrDefault();
            
            if (lastMonth == null) return false;

            var lastDate = DateTime.Parse($"{lastMonth.Month}-01");
            var newDate = lastDate.AddMonths(1);
            var newMonth = newDate.ToString("yyyy-MM");

            var existingMonth = await _database.GetMonthlyDataAsync(newMonth);
            if (existingMonth != null) return false;

            var defaultBudget = (await _database.GetAllBudgetsAsync())
                .FirstOrDefault(b => b.IsDefault);
            
            var defaultIncome = 0m;
            if (defaultBudget != null)
            {
                var incomeCategory = (await _database.GetCategoriesByBudgetIdAsync(defaultBudget.Id))
                    .FirstOrDefault(c => c.Type == "income");
                defaultIncome = incomeCategory?.Amount ?? 0;
            }

            var newMonthlyData = new Schema.MonthlyData
            {
                Month = newMonth,
                Income = defaultIncome,
                BudgetId = defaultBudget?.Id
            };

            await _database.InsertMonthlyDataAsync(newMonthlyData);
            return true;
        }

        public async Task SeedInitialData()
        {
            await InitializeDatabase();

            // Check if data already exists
            var budgetCount = await _database.GetBudgetCountAsync();
            if (budgetCount == 0)
            {
                var initialBudgets = GetInitialBudgets();
                await _database.InsertAllBudgetsAsync(initialBudgets);

                // Insert categories with proper foreign keys
                var categories = new List<Schema.BudgetCategory>();
                foreach (var budget in initialBudgets)
                {
                    foreach (var category in GetCategoriesForBudget(budget))
                    {
                        category.BudgetId = budget.Id;
                        categories.Add(category);
                    }
                }
                await _database.InsertAllCategoriesAsync(categories);
            }

            var monthCount = await _database.GetMonthlyDataCountAsync();
            if (monthCount == 0)
            {
                var yearData = InitializeYearData(DateTime.Now.Year);
                await _database.InsertAllMonthlyDataAsync(yearData.Values);
            }
        }

        private Dictionary<string, Schema.MonthlyData> InitializeYearData(int year)
        {
            var yearData = new Dictionary<string, Schema.MonthlyData>();
            var defaultBudget = GetInitialBudgets().FirstOrDefault(b => b.IsDefault);
            var defaultIncome = GetCategoriesForBudget(defaultBudget)
                .FirstOrDefault(c => c.Type == "income")?.Amount ?? 5000;

            for (int i = 0; i < 12; i++)
            {
                var date = new DateTime(year, 1, 1).AddMonths(i);
                var monthKey = date.ToString("yyyy-MM");
                yearData[monthKey] = new Schema.MonthlyData
                {
                    Month = monthKey,
                    Income = defaultIncome,
                    BudgetId = defaultBudget?.Id
                };
            }

            return yearData;
        }

        private List<Schema.Budget> GetInitialBudgets()
        {
            return new List<Schema.Budget>
            {
                new Schema.Budget
                {
                    Id = Guid.NewGuid().ToString(),
                    Name = "Basic Budget",
                    IsDefault = true,
                    Description = "A simple budget for getting started"
                },
                new Schema.Budget
                {
                    Id = Guid.NewGuid().ToString(),
                    Name = "Frugal Budget",
                    IsDefault = false,
                    Description = "A budget focused on maximizing savings"
                }
            };
        }

        private List<Schema.BudgetCategory> GetCategoriesForBudget(Schema.Budget budget)
        {
            if (budget == null) return new List<Schema.BudgetCategory>();

            if (budget.Name == "Basic Budget")
            {
                return new List<Schema.BudgetCategory>
                {
                    new Schema.BudgetCategory { BudgetId = budget.Id, Name = "Income", Type = "income", Amount = 5000, IsRecurring = true, DayOfMonth = 1 },
                    new Schema.BudgetCategory { BudgetId = budget.Id, Name = "Housing", Type = "expense", Amount = 1500, IsRecurring = true, DayOfMonth = 1 },
                    new Schema.BudgetCategory { BudgetId = budget.Id, Name = "Food", Type = "expense", Amount = 500, IsRecurring = false },
                    new Schema.BudgetCategory { BudgetId = budget.Id, Name = "Utilities", Type = "expense", Amount = 300, IsRecurring = true, DayOfMonth = 15 },
                    new Schema.BudgetCategory { BudgetId = budget.Id, Name = "Transportation", Type = "expense", Amount = 200, IsRecurring = false },
                    new Schema.BudgetCategory { BudgetId = budget.Id, Name = "Entertainment", Type = "expense", Amount = 200, IsRecurring = false },
                    new Schema.BudgetCategory { BudgetId = budget.Id, Name = "Savings", Type = "savings", Amount = 500, IsRecurring = true, DayOfMonth = 1 }
                };
            }
            else if (budget.Name == "Frugal Budget")
            {
                return new List<Schema.BudgetCategory>
                {
                    new Schema.BudgetCategory { BudgetId = budget.Id, Name = "Income", Type = "income", Amount = 5000, IsRecurring = true, DayOfMonth = 1 },
                    new Schema.BudgetCategory { BudgetId = budget.Id, Name = "Housing", Type = "expense", Amount = 1200, IsRecurring = true, DayOfMonth = 1 },
                    new Schema.BudgetCategory { BudgetId = budget.Id, Name = "Food", Type = "expense", Amount = 300, IsRecurring = false },
                    new Schema.BudgetCategory { BudgetId = budget.Id, Name = "Utilities", Type = "expense", Amount = 250, IsRecurring = true, DayOfMonth = 15 },
                    new Schema.BudgetCategory { BudgetId = budget.Id, Name = "Transportation", Type = "expense", Amount = 150, IsRecurring = false },
                    new Schema.BudgetCategory { BudgetId = budget.Id, Name = "Entertainment", Type = "expense", Amount = 100, IsRecurring = false },
                    new Schema.BudgetCategory { BudgetId = budget.Id, Name = "Savings", Type = "savings", Amount = 1000, IsRecurring = true, DayOfMonth = 1 }
                };
            }

            return new List<Schema.BudgetCategory>();
        }
    }
}