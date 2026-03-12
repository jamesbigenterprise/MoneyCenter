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

        public static Schema.Expense ToSchema(this ViewModel.Objects.Expense expense)
        {
            if (expense == null) return null;
            return new Schema.Expense
            {
                Id = expense.Id,
                Amount = expense.Amount,
                Date = expense.Date,
                Category = expense.Category,
                Destination = expense.Destination,
                Details = expense.Details
            };
        }

        
    }
}