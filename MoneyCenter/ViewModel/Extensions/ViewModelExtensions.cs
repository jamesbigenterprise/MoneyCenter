using System.Linq;
using System.Collections.Generic;

namespace MoneyCenter.ViewModel.Extensions
{
    public static class ViewModelExtensions
    {
        // To ViewModel - Budget
        public static ViewModel.Objects.Budget ToViewModel(this Schema.Budget budget, List<Schema.BudgetMasterCategory> categories = null)
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

        public static ViewModel.Objects.BudgetCategory ToViewModel(this Schema.BudgetMasterCategory assignment)
        {
            if (assignment == null) return null;
            return new ViewModel.Objects.BudgetCategory
            {
                Id = assignment.MasterCategoryId,
                Name = assignment.Id.ToString(), // Will need to be replaced with actual category name
                Amount = assignment.Amount,
                Type = string.Empty,
                IsRecurring = assignment.IsRecurring,
                DayOfMonth = assignment.DayOfMonth
            };
        }

        public static ViewModel.Objects.MasterCategory ToViewModel(this Schema.MasterCategory category)
        {
            if (category == null) return null;
            return new ViewModel.Objects.MasterCategory
            {
                Id = category.Id,
                Name = category.Name,
                Type = category.Type,
                Description = category.Description
            };
        }

        public static ViewModel.Objects.CategoryType ToViewModel(this Schema.CategoryType type)
        {
            if (type == null) return null;
            return new ViewModel.Objects.CategoryType
            {
                Key = type.Key,
                Name = type.Name,
                Color = type.Color,
                TextColor = type.TextColor,
                IsSystem = type.IsSystem
            };
        }

        public static ViewModel.Objects.Budget ToViewModel(this Schema.Budget budget)
        {
            if (budget == null) return null;
            return new ViewModel.Objects.Budget
            {
                Id = budget.Id,
                Name = budget.Name,
                Description = budget.Description,
                IsDefault = budget.IsDefault,
                Categories = new List<ViewModel.Objects.BudgetCategory>()
            };
        }

        public static ViewModel.Objects.Expense ToViewModel(this Schema.Expense expense)
        {
            if (expense == null) return null;
            return new ViewModel.Objects.Expense
            {
                Id = expense.Id,
                MonthId = expense.MonthId,
                Amount = expense.Amount,
                Date = expense.Date,
                MasterCategoryId = expense.MasterCategoryId,
                Destination = expense.Destination,
                Details = expense.Details,
                PaymentAccountId = expense.PaymentAccountId,
                BudgetId = expense.BudgetId,
                SavingsPodId = expense.SavingsPodId
            };
        }

        public static ViewModel.Objects.PaymentAccount ToViewModel(this Schema.PaymentAccount account)
        {
            if (account == null) return null;
            return new ViewModel.Objects.PaymentAccount
            {
                Id = account.Id,
                Name = account.Name,
                Type = account.Type,
                Description = account.Description
            };
        }

        // To Schema
        public static Schema.BudgetMasterCategory ToSchema(this ViewModel.Objects.BudgetCategory category, string budgetId = null)
        {
            if (category == null) return null;
            return new Schema.BudgetMasterCategory
            {
                BudgetId = budgetId ?? string.Empty,
                MasterCategoryId = category.Id,
                Amount = category.Amount ?? 0,
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
                MonthId = expense.MonthId,
                Amount = expense.Amount,
                Date = expense.Date,
                MasterCategoryId = expense.MasterCategoryId,
                Destination = expense.Destination,
                Details = expense.Details,
                PaymentAccountId = expense.PaymentAccountId,
                BudgetId = expense.BudgetId,
                SavingsPodId = expense.SavingsPodId
            };
        }
    }
}
