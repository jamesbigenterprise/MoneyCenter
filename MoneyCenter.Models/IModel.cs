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

        Task AddEntry(SingleEntryDataModel entry);

        Task<Dictionary<string, MonthlyData>> GetAllData();

        Task<List<Budget>> GetBudgets();

        Task<List<string>> GetAllCategories();

        Task<List<BudgetCategory>> GetMasterCategories();
        
        Task<List<BudgetCategory>> GetCategoriesByBudgetId(string budgetId);
        
        Task<List<Expense>> GetExpensesByMonth(string month);
        
        Task<List<Expense>> GetAllExpenses();

        Task AddCategoryAsync(BudgetCategory category);

        Task AddExpense(string month, Expense expense);

        Task UpdateExpense(string month, Expense expense);

        Task DeleteExpense(string month, string expenseId);

        Task SetBudgetForMonth(string budgetId, string month);

        Task SetIncomeForMonth(decimal income, string month);

        Task<bool> AddNewMonth();

        Task SeedInitialData();
    }
}