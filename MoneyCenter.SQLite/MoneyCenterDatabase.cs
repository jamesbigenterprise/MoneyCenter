using Microsoft.VisualBasic;
using MoneyCenter.Schema;
using SQLite;
using System.Threading.Tasks;

namespace MoneyCenter.SQLiteWrapper
{
    //figure out initialization logic
    // Some separate entities in the db are tightly coupled so I would not wand isolated pieces floating like a moth without a year
    // The only single add function available should be the full 12 months and the year
    //
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
              
            await _database.CreateTableAsync<Budget>();
            await _database.CreateTableAsync<BudgetCategory>();
            await _database.CreateTableAsync<Expense>();
            await _database.CreateTableAsync<Year>();
            await _database.CreateTableAsync<Month>();
        }

        // Year CRUD
        public async Task<int> AddYearAsync(int yearValue)
        {
            var existing = await _database.Table<Year>().FirstOrDefaultAsync(y => y.YearValue == yearValue);
            if (existing != null)
                return existing.Id;
            var year = new Year { YearValue = yearValue };
            await _database.InsertAsync(year);
            return year.Id;
        }

        public async Task<Year> GetYearByIdAsync(int yearId)
        {
            return await _database.Table<Year>().FirstOrDefaultAsync(y => y.Id == yearId);
        }

        // SingleEntryDataModel CRUD
      
       

     

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

        public async Task<List<Expense>> GetExpensesByMonthAsync(int monthId)
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

      
        // Helper methods
        public async Task<int> GetBudgetCountAsync()
        {
            return await _database.Table<Budget>().CountAsync();
        }

        public async Task<int?> GetMonthIdAsync(string month, int year)
        {
            var yearRecord = await _database.Table<Year>().FirstOrDefaultAsync(y => y.YearValue == year);
            if (yearRecord == null) return null;
            return (await _database.Table<Month>()
                .FirstOrDefaultAsync(m => m.MonthName == month && m.YearId == yearRecord.Id))?.Id;
        }

        public async Task<int?> CreateMonthAsync(string currentMonth, int currentYear)
        {
            // Ensure year exists
            int yearId = await AddYearAsync(currentYear);
            // Ensure not existing and create
            int? existingMonth = await GetMonthIdAsync(currentMonth, currentYear);
            if (existingMonth == null)
            {
                var newMonth = new Month
                {
                    MonthName = currentMonth,
                    MonthNumber = DateTime.ParseExact(currentMonth, "MMMM", null).Month,
                    YearId = yearId
                };
                await _database.InsertAsync(newMonth);
                //return month id
                return newMonth.Id;
            }
            return existingMonth;
        }

        public async Task<int> AddYearWithMonthsAsync(int yearValue)
        {
            // Check if year exists
            var existingYear = await _database.Table<Year>().FirstOrDefaultAsync(y => y.YearValue == yearValue);
            if (existingYear != null)
                return existingYear.Id;

            // Create year
            var year = new Year { YearValue = yearValue };
            await _database.InsertAsync(year);

            // Create 12 months for this year
            var monthNames = System.Globalization.CultureInfo.CurrentCulture.DateTimeFormat.MonthNames;
            for (int i = 1; i <= 12; i++)
            {
                var monthName = monthNames[i - 1];
                if (string.IsNullOrEmpty(monthName)) continue; // skip empty
                var newMonth = new Month
                {
                    YearId = year.Id,
                    MonthNumber = i,
                    MonthName = monthName
                };
                await _database.InsertAsync(newMonth);
            }
            return year.Id;
        }

        public async Task<List<Month>> GetMonthsByYearAsync(int yearId)
        {
            return await _database.Table<Month>()
                .Where(m => m.YearId == yearId)
                .OrderBy(m => m.MonthNumber)
                .ToListAsync();
        }

        public async Task<List<Year>> GetAllYearsAsync()
        {
            return await _database.Table<Year>()
                .OrderBy(y => y.YearValue)
                .ToListAsync();
        }

        public async Task AddYearWithMonths(int yearValue)
        {
            await AddYearWithMonthsAsync(yearValue);
        }

        public async Task<List<Expense>> GetExpensesByMonthIdAsync(int monthId)
        {
            return await _database.Table<Expense>()
                .Where(e => e.MonthId == monthId)
                .ToListAsync();
        }
    }
}
