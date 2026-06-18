using MoneyCenter.Schema;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoneyCenter.Model
{
    public interface IModel
    {
        Task InitializeDatabase();

        // Year operations
        Task<List<Year>> GetYears();
        Task AddYear(int yearValue);

        // Month operations
        Task<List<Month>> GetMonthsByYear(int yearId);
        Task<int> AssignBudgetToMonth(int monthId, string budgetId);
        Task<Budget> GetBudgetForMonth(int monthId);

        // Budget operations
        Task<List<Budget>> GetBudgets();
        Task AddBudget(Budget budget);
        Task UpdateBudget(Budget budget);
        Task DeleteBudget(string budgetId);

        // Category type operations
        Task<List<CategoryType>> GetCategoryTypes();
        Task AddCategoryType(CategoryType type);
        Task UpdateCategoryType(CategoryType type);
        Task DeleteCategoryType(string key);

        // MasterCategory operations (centralized categories)
        Task<List<MasterCategory>> GetMasterCategories();
        Task<MasterCategory> GetMasterCategoryById(int categoryId);
        Task<List<MasterCategory>> GetMasterCategoriesByType(string type);
        Task AddMasterCategory(MasterCategory category);
        Task UpdateMasterCategory(MasterCategory category);
        Task DeleteMasterCategory(int categoryId);

        // Budget-Category assignments
        Task<List<BudgetMasterCategory>> GetCategoriesByBudgetId(string budgetId);
        Task AssignCategoryToBudget(string budgetId, int masterCategoryId, decimal amount, bool isRecurring, int? dayOfMonth);
        Task RemoveCategoryFromBudget(string budgetId, int masterCategoryId);
        Task UpdateBudgetCategoryAssignment(string budgetId, int masterCategoryId, decimal amount, bool isRecurring, int? dayOfMonth);

        // PaymentAccount operations
        Task<List<PaymentAccount>> GetPaymentAccounts();
        Task AddPaymentAccount(PaymentAccount account);
        Task UpdatePaymentAccount(PaymentAccount account);
        Task DeletePaymentAccount(int accountId);

        // SavingsPod operations
        Task<List<SavingsPod>> GetAllSavingsPods();
        Task<SavingsPod> GetSavingsPodByMasterCategoryId(int masterCategoryId);
        Task CreateSavingsPod(int masterCategoryId);
        Task<decimal> GetSavingsPodBalance(int savingsPodId);
        Task TransferExpenseToSavingsPod(string expenseId, int savingsPodId);

        // Expense operations
        Task<List<Expense>> GetExpensesByMonthId(int monthId);
        Task AddExpense(int monthId, Expense expense);
        Task UpdateExpense(int monthId, Expense expense);
        Task DeleteExpense(int monthId, string expenseId);
        Task<Expense> GetExpense(string expenseId);

        // Recurring expense logic
        Task ProcessRecurringExpenses(int monthId);

        // Initialization/Seeding
        Task SeedInitialData();
    }
}
