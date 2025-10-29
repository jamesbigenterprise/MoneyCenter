using MoneyCenter.ViewModel.Objects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoneyCenter.Services
{
    public interface IFinancialService
    {
        Dictionary<string, MonthlyData> GetAllData();
        List<Budget> GetBudgets();
        List<string> GetAllCategories();
        Task AddCategoryAsync(BudgetCategory category);
        Task AddExpense(string month, Expense expense);
        Task UpdateExpense(string month, Expense expense);
        Task DeleteExpense(string month, string expenseId);
        void SetBudgetForMonth(string budgetId, string month);
        void SetIncomeForMonth(decimal income, string month);
        Task<bool> AddNewMonth();
        Dictionary<string, MonthlyData> InitializeYearData(int year);
        List<Budget> GetInitialBudgets();
        List<BudgetCategory> GetMasterCategories();
    }
}
