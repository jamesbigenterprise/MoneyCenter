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
            await _database.CreateTableAsync<MasterCategory>();
            await _database.CreateTableAsync<BudgetMasterCategory>();
            await _database.CreateTableAsync<PaymentAccount>();
            await _database.CreateTableAsync<SavingsPod>();
            await _database.CreateTableAsync<Expense>();
            await _database.CreateTableAsync<Year>();
            await _database.CreateTableAsync<Month>();
            // Keep old BudgetCategory for now during transition
            await _database.CreateTableAsync<BudgetCategory>();
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

        public async Task<int> InsertAllMasterCategoriesAsync(IEnumerable<MasterCategory> categories)
        {
            return await _database.InsertAllAsync(categories);
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

        // MasterCategory CRUD
        public async Task<List<MasterCategory>> GetAllMasterCategoriesAsync()
        {
            return await _database.Table<MasterCategory>().ToListAsync();
        }

        public async Task<MasterCategory> GetMasterCategoryByIdAsync(int id)
        {
            return await _database.Table<MasterCategory>().FirstOrDefaultAsync(c => c.Id == id);
        }

        public async Task<MasterCategory> GetMasterCategoryByNameAsync(string name)
        {
            return await _database.Table<MasterCategory>()
                .FirstOrDefaultAsync(c => c.Name.ToUpper() == name.ToUpper());
        }

        public async Task<List<MasterCategory>> GetMasterCategoriesByTypeAsync(string type)
        {
            return await _database.Table<MasterCategory>()
                .Where(c => c.Type == type)
                .ToListAsync();
        }

        public async Task<int> InsertMasterCategoryAsync(MasterCategory category)
        {
            return await _database.InsertAsync(category);
        }

        public async Task<int> UpdateMasterCategoryAsync(MasterCategory category)
        {
            return await _database.UpdateAsync(category);
        }

        public async Task<int> DeleteMasterCategoryAsync(MasterCategory category)
        {
            return await _database.DeleteAsync(category);
        }

        // BudgetMasterCategory (Join table) CRUD
        public async Task<List<BudgetMasterCategory>> GetCategoriesByBudgetIdNewAsync(string budgetId)
        {
            return await _database.Table<BudgetMasterCategory>()
                .Where(bc => bc.BudgetId == budgetId)
                .ToListAsync();
        }

        public async Task<List<BudgetMasterCategory>> GetBudgetsByMasterCategoryAsync(int masterCategoryId)
        {
            return await _database.Table<BudgetMasterCategory>()
                .Where(bc => bc.MasterCategoryId == masterCategoryId)
                .ToListAsync();
        }

        public async Task<BudgetMasterCategory> GetBudgetCategoryAssignmentAsync(string budgetId, int masterCategoryId)
        {
            return await _database.Table<BudgetMasterCategory>()
                .FirstOrDefaultAsync(bc => bc.BudgetId == budgetId && bc.MasterCategoryId == masterCategoryId);
        }

        public async Task<int> InsertBudgetMasterCategoryAsync(BudgetMasterCategory assignment)
        {
            return await _database.InsertAsync(assignment);
        }

        public async Task<int> UpdateBudgetMasterCategoryAsync(BudgetMasterCategory assignment)
        {
            return await _database.UpdateAsync(assignment);
        }

        public async Task<int> DeleteBudgetMasterCategoryAsync(BudgetMasterCategory assignment)
        {
            return await _database.DeleteAsync(assignment);
        }

        public async Task<int> RemoveBudgetCategoryAssignmentAsync(string budgetId, int masterCategoryId)
        {
            var assignment = await GetBudgetCategoryAssignmentAsync(budgetId, masterCategoryId);
            if (assignment != null)
                return await _database.DeleteAsync(assignment);
            return 0;
        }

        // PaymentAccount CRUD
        public async Task<List<PaymentAccount>> GetAllPaymentAccountsAsync()
        {
            return await _database.Table<PaymentAccount>().ToListAsync();
        }

        public async Task<PaymentAccount> GetPaymentAccountByIdAsync(int id)
        {
            return await _database.Table<PaymentAccount>().FirstOrDefaultAsync(a => a.Id == id);
        }

        public async Task<PaymentAccount> GetPaymentAccountByNameAsync(string name)
        {
            return await _database.Table<PaymentAccount>()
                .FirstOrDefaultAsync(a => a.Name.ToUpper() == name.ToUpper());
        }

        public async Task<int> InsertPaymentAccountAsync(PaymentAccount account)
        {
            return await _database.InsertAsync(account);
        }

        public async Task<int> UpdatePaymentAccountAsync(PaymentAccount account)
        {
            return await _database.UpdateAsync(account);
        }

        public async Task<int> DeletePaymentAccountAsync(PaymentAccount account)
        {
            return await _database.DeleteAsync(account);
        }

        // SavingsPod CRUD
        public async Task<List<SavingsPod>> GetAllSavingsPodsAsync()
        {
            return await _database.Table<SavingsPod>().ToListAsync();
        }

        public async Task<SavingsPod> GetSavingsPodByIdAsync(int id)
        {
            return await _database.Table<SavingsPod>().FirstOrDefaultAsync(sp => sp.Id == id);
        }

        public async Task<SavingsPod> GetSavingsPodByMasterCategoryIdAsync(int masterCategoryId)
        {
            return await _database.Table<SavingsPod>()
                .FirstOrDefaultAsync(sp => sp.MasterCategoryId == masterCategoryId);
        }

        public async Task<int> InsertSavingsPodAsync(SavingsPod pod)
        {
            return await _database.InsertAsync(pod);
        }

        public async Task<int> UpdateSavingsPodAsync(SavingsPod pod)
        {
            return await _database.UpdateAsync(pod);
        }

        public async Task<int> DeleteSavingsPodAsync(SavingsPod pod)
        {
            return await _database.DeleteAsync(pod);
        }

        public async Task<List<Expense>> GetSavingsPodExpensesAsync(int savingsPodId)
        {
            return await _database.Table<Expense>()
                .Where(e => e.SavingsPodId == savingsPodId)
                .ToListAsync();
        }

        public async Task<decimal> GetSavingsPodBalanceAsync(int savingsPodId)
        {
            var expenses = await GetSavingsPodExpensesAsync(savingsPodId);
            return expenses.Sum(e => e.Amount);
        }

        // Month-Budget Assignment
        public async Task<int> AssignBudgetToMonthAsync(int monthId, string budgetId)
        {
            var month = await _database.Table<Month>().FirstOrDefaultAsync(m => m.Id == monthId);
            if (month != null)
            {
                month.BudgetId = budgetId;
                return await _database.UpdateAsync(month);
            }
            return 0;
        }

        public async Task<Budget> GetBudgetForMonthAsync(int monthId)
        {
            var month = await _database.Table<Month>().FirstOrDefaultAsync(m => m.Id == monthId);
            if (month != null && !string.IsNullOrEmpty(month.BudgetId))
            {
                return await GetBudgetAsync(month.BudgetId);
            }
            return null;
        }

        // Recurring Expense Logic
        public async Task<List<BudgetMasterCategory>> GetRecurringCategoriesForBudgetAsync(string budgetId)
        {
            return await _database.Table<BudgetMasterCategory>()
                .Where(bc => bc.BudgetId == budgetId && bc.IsRecurring)
                .ToListAsync();
        }

        public async Task<bool> ShouldCreateRecurringExpenseAsync(int masterCategoryId, int dayOfMonth, int currentDay)
        {
            var category = await GetMasterCategoryByIdAsync(masterCategoryId);
            if (category == null) return false;

            var assignments = await GetBudgetsByMasterCategoryAsync(masterCategoryId);
            var recurringAssignment = assignments.FirstOrDefault(a => a.IsRecurring && a.DayOfMonth == dayOfMonth);
            
            return recurringAssignment != null && dayOfMonth == currentDay;
        }
    }
}
