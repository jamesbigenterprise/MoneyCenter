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
                throw new ArgumentNullException(nameof(database));
            }
            _database = database;
        }

        public async Task InitializeDatabase()
        {
            await _database.InitializeAsync();
        }

        // Lookup month by name and year
        public async Task<int?> GetMonthIdByName(string monthName, int yearId)
        {
            await InitializeDatabase();
            var months = await _database.GetMonthsByYearAsync(yearId);
            var match = months.FirstOrDefault(m => m.MonthName.Equals(monthName, StringComparison.OrdinalIgnoreCase));
            return match?.Id;
        }

        // Lookup month by number and year
        public async Task<int?> GetMonthIdByNumber(int monthNumber, int yearId)
        {
            await InitializeDatabase();
            var months = await _database.GetMonthsByYearAsync(yearId);
            var match = months.FirstOrDefault(m => m.MonthNumber == monthNumber);
            return match?.Id;
        }

        public async Task<List<Expense>> GetExpensesByMonthId(int monthId)
        {
            await InitializeDatabase();
            return await _database.GetExpensesByMonthIdAsync(monthId);
        }

        public async Task AddExpense(int monthId, Expense expense)
        {
            await InitializeDatabase();
            expense.MonthId = monthId;
            await _database.InsertExpenseAsync(expense);
        }

        public async Task UpdateExpense(int monthId, Schema.Expense expense)
        {
            await InitializeDatabase();
            
            var existingExpense = await _database.GetExpenseAsync(expense.Id);
            if (existingExpense != null)
            {
                expense.MonthId = monthId;
                await _database.UpdateExpenseAsync(expense);
            }
        }

        public async Task DeleteExpense(int monthId, string expenseId)
        {
            await InitializeDatabase();
            await _database.DeleteExpenseByIdAsync(expenseId);
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

            // Add current year with all 12 months if it doesn't exist
            int currentYear = DateTime.Now.Year;
            await _database.AddYearWithMonthsAsync(currentYear);
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

        // Hierarchical methods for year/month/expense
        public async Task<List<Schema.Year>> GetYears()
        {
            await InitializeDatabase();
            return await _database.GetAllYearsAsync();
        }

        public async Task<List<Schema.Month>> GetMonthsByYear(int yearId)
        {
            await InitializeDatabase();
            return await _database.GetMonthsByYearAsync(yearId);
        }

        public async Task AddYear(int yearValue)
        {
            await InitializeDatabase();
            await _database.AddYearAsync(yearValue);
        }

        public Task<Dictionary<string, Expense>> GetAllData()
        {
            throw new NotImplementedException();
        }

        public async Task<List<Budget>> GetBudgets()
        {
            await InitializeDatabase();
            return await _database.GetAllBudgetsAsync();
        }

        public Task<List<string>> GetAllCategories()
        {
            throw new NotImplementedException();
        }

        public Task<List<BudgetCategory>> GetMasterCategories()
        {
            throw new NotImplementedException();
        }

        public Task<List<BudgetCategory>> GetCategoriesByBudgetId(string budgetId)
        {
            throw new NotImplementedException();
        }

        public Task<List<Expense>> GetExpensesByMonth(string month)
        {
            throw new NotImplementedException();
        }

        public Task<List<Expense>> GetAllExpenses()
        {
            throw new NotImplementedException();
        }

        public Task AddCategoryAsync(BudgetCategory category)
        {
            throw new NotImplementedException();
        }

        public Task AddExpense(string month, Expense expense)
        {
            throw new NotImplementedException();
        }

        public Task UpdateExpense(string month, Expense expense)
        {
            throw new NotImplementedException();
        }

        public Task DeleteExpense(string month, string expenseId)
        {
            throw new NotImplementedException();
        }

        public Task SetBudgetForMonth(string budgetId, string month)
        {
            throw new NotImplementedException();
        }

        public Task SetIncomeForMonth(decimal income, string month)
        {
            throw new NotImplementedException();
        }

        public Task<bool> AddNewMonth()
        {
            throw new NotImplementedException();
        }
    }
}