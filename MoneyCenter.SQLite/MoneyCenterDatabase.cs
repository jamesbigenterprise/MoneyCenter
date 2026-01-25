using Microsoft.VisualBasic;
using MoneyCenter.Schema;
using SQLite;

namespace MoneyCenter.SQLiteWrapper
{
    public class MoneyCenterDatabase
    {
        private SQLiteAsyncConnection _database;

        public MoneyCenterDatabase()
        {
            if (_database != null)
                return;
            _database = new SQLiteAsyncConnection(DatabaseConfig.DatabasePath, DatabaseConfig.Flags);
        }

        public async Task InitializeAsync()
        {
            if (_database == null) 
            {
                _database = new SQLiteAsyncConnection(DatabaseConfig.DatabasePath, DatabaseConfig.Flags);
            }
              

            await _database.CreateTableAsync<SingleEntryDataModel>();
            await _database.CreateTableAsync<Budget>();
            await _database.CreateTableAsync<BudgetCategory>();
            await _database.CreateTableAsync<Expense>();
            await _database.CreateTableAsync<MonthlyData>();
        }

        // SingleEntryDataModel CRUD
        public async Task<List<SingleEntryDataModel>> GetAllEntries()
        {
            return await _database.Table<SingleEntryDataModel>().ToListAsync();
        }

        public async Task<int> DeleteEntryByID(int id)
        {
            return await _database.Table<SingleEntryDataModel>().DeleteAsync(entry => entry.Id == id);
        }

        public async Task<int> InsertEntry(SingleEntryDataModel entry)
        {
            return await _database.InsertAsync(entry);
        }

        // Budget CRUD
        public async Task<List<Budget>> GetAllBudgetsAsync()
        {
            return await _database.Table<Budget>().ToListAsync();
        }

        public async Task<Budget> GetBudgetAsync(string id)
        {
            return await _database.Table<Budget>().FirstOrDefaultAsync(b => b.Id == id);
        }

        public async Task<int> InsertBudgetAsync(Budget budget)
        {
            return await _database.InsertAsync(budget);
        }

        public async Task<int> InsertAllBudgetsAsync(IEnumerable<Budget> budgets)
        {
            return await _database.InsertAllAsync(budgets);
        }

        public async Task<int> UpdateBudgetAsync(Budget budget)
        {
            return await _database.UpdateAsync(budget);
        }

        public async Task<int> DeleteBudgetAsync(Budget budget)
        {
            return await _database.DeleteAsync(budget);
        }

        // BudgetCategory CRUD
        public async Task<List<BudgetCategory>> GetAllCategoriesAsync()
        {
            return await _database.Table<BudgetCategory>().ToListAsync();
        }

        public async Task<List<BudgetCategory>> GetCategoriesByBudgetIdAsync(string budgetId)
        {
            return await _database.Table<BudgetCategory>()
                .Where(c => c.BudgetId == budgetId)
                .ToListAsync();
        }

        public async Task<BudgetCategory> GetCategoryAsync(int id)
        {
            return await _database.Table<BudgetCategory>().FirstOrDefaultAsync(c => c.Id == id);
        }

        public async Task<BudgetCategory> GetCategoryByNameAsync(string name)
        {
            return await _database.Table<BudgetCategory>().FirstOrDefaultAsync(c => c.Name == name);
        }

        public async Task<int> InsertCategoryAsync(BudgetCategory category)
        {
            return await _database.InsertAsync(category);
        }

        public async Task<int> InsertAllCategoriesAsync(IEnumerable<BudgetCategory> categories)
        {
            return await _database.InsertAllAsync(categories);
        }

        public async Task<int> UpdateCategoryAsync(BudgetCategory category)
        {
            return await _database.UpdateAsync(category);
        }

        public async Task<int> DeleteCategoryAsync(BudgetCategory category)
        {
            return await _database.DeleteAsync(category);
        }

        // Expense CRUD
        public async Task<List<Expense>> GetAllExpensesAsync()
        {
            return await _database.Table<Expense>().ToListAsync();
        }

        public async Task<List<Expense>> GetExpensesByMonthAsync(string monthId)
        {
            return await _database.Table<Expense>()
                .Where(e => e.MonthId == monthId)
                .ToListAsync();
        }

        public async Task<Expense> GetExpenseAsync(string id)
        {
            return await _database.Table<Expense>().FirstOrDefaultAsync(e => e.Id == id);
        }

        public async Task<int> InsertExpenseAsync(Expense expense)
        {
            return await _database.InsertAsync(expense);
        }

        public async Task<int> InsertAllExpensesAsync(IEnumerable<Expense> expenses)
        {
            return await _database.InsertAllAsync(expenses);
        }

        public async Task<int> UpdateExpenseAsync(Expense expense)
        {
            return await _database.UpdateAsync(expense);
        }

        public async Task<int> DeleteExpenseAsync(Expense expense)
        {
            return await _database.DeleteAsync(expense);
        }

        public async Task<int> DeleteExpenseByIdAsync(string id)
        {
            var expense = await GetExpenseAsync(id);
            if (expense != null)
                return await _database.DeleteAsync(expense);
            return 0;
        }

        // MonthlyData CRUD
        public async Task<List<MonthlyData>> GetAllMonthlyDataAsync()
        {
            return await _database.Table<MonthlyData>().ToListAsync();
        }

        public async Task<MonthlyData> GetMonthlyDataAsync(string month)
        {
            return await _database.Table<MonthlyData>().FirstOrDefaultAsync(m => m.Month == month);
        }

        public async Task<int> InsertMonthlyDataAsync(MonthlyData data)
        {
            return await _database.InsertAsync(data);
        }

        public async Task<int> InsertAllMonthlyDataAsync(IEnumerable<MonthlyData> dataList)
        {
            return await _database.InsertAllAsync(dataList);
        }

        public async Task<int> UpdateMonthlyDataAsync(MonthlyData data)
        {
            return await _database.UpdateAsync(data);
        }

        public async Task<int> DeleteMonthlyDataAsync(MonthlyData data)
        {
            return await _database.DeleteAsync(data);
        }

        // Helper methods
        public async Task<int> GetBudgetCountAsync()
        {
            return await _database.Table<Budget>().CountAsync();
        }

        public async Task<int> GetMonthlyDataCountAsync()
        {
            return await _database.Table<MonthlyData>().CountAsync();
        }
    }
}
