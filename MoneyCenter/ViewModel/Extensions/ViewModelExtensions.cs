using System.Linq;
using System.Collections.Generic;

namespace MoneyCenter.ViewModel.Extensions
{
    public static class ViewModelExtensions
    {
        // To ViewModel
        public static ViewModel.Objects.Budget ToViewModel(this Schema.Budget budget, List<Schema.BudgetCategory> categories = null)
        {
            if (budget == null) return null;
            return new ViewModel.Objects.Budget
            {
                Id = budget.Id,
                Name = budget.Name,
                Description = budget.Description,
                IsDefault = budget.IsDefault,
                Categories = categories?.Where(c => c.BudgetId == budget.Id)
                    .Select(c => c.ToViewModel())
                    .ToList() ?? new List<ViewModel.Objects.BudgetCategory>()
            };
        }

        public static ViewModel.Objects.BudgetCategory ToViewModel(this Schema.BudgetCategory category)
        {
            if (category == null) return null;
            return new ViewModel.Objects.BudgetCategory
            {
                Name = category.Name,
                Amount = category.Amount,
                Type = category.Type,
                IsRecurring = category.IsRecurring,
                DayOfMonth = category.DayOfMonth
            };
        }

        public static ViewModel.Objects.Expense ToViewModel(this Schema.Expense expense)
        {
            if (expense == null) return null;
            return new ViewModel.Objects.Expense
            {
                Id = expense.Id,
                Amount = expense.Amount,
                Date = expense.Date,
                Category = expense.Category,
                Destination = expense.Destination,
                Details = expense.Details
            };
        }

        public static ViewModel.Objects.MonthlyData ToViewModel(this Schema.MonthlyData monthlyData, List<Schema.Expense> expenses = null)
        {
            if (monthlyData == null) return null;
            return new ViewModel.Objects.MonthlyData
            {
                Month = monthlyData.Month,
                Income = monthlyData.Income,
                BudgetId = monthlyData.BudgetId,
                Expenses = expenses?.Where(e => e.MonthId == monthlyData.Month)
                    .Select(e => e.ToViewModel())
                    .ToList() ?? new List<ViewModel.Objects.Expense>()
            };
        }

        // To Schema
        public static Schema.Budget ToSchema(this ViewModel.Objects.Budget budget)
        {
            if (budget == null) return null;
            return new Schema.Budget
            {
                Id = budget.Id,
                Name = budget.Name,
                Description = budget.Description,
                IsDefault = budget.IsDefault
            };
        }

        public static Schema.BudgetCategory ToSchema(this ViewModel.Objects.BudgetCategory category, string budgetId = null)
        {
            if (category == null) return null;
            return new Schema.BudgetCategory
            {
                BudgetId = budgetId ?? string.Empty,
                Name = category.Name,
                Amount = category.Amount,
                Type = category.Type,
                IsRecurring = category.IsRecurring,
                DayOfMonth = category.DayOfMonth
            };
        }

        public static Schema.Expense ToSchema(this ViewModel.Objects.Expense expense, string monthId = null)
        {
            if (expense == null) return null;
            return new Schema.Expense
            {
                Id = expense.Id,
                MonthId = monthId ?? string.Empty,
                Amount = expense.Amount,
                Date = expense.Date,
                Category = expense.Category,
                Destination = expense.Destination,
                Details = expense.Details
            };
        }

        public static Schema.MonthlyData ToSchema(this ViewModel.Objects.MonthlyData monthlyData)
        {
            if (monthlyData == null) return null;
            return new Schema.MonthlyData
            {
                Month = monthlyData.Month,
                Income = monthlyData.Income,
                BudgetId = monthlyData.BudgetId
            };
        }
    }
}