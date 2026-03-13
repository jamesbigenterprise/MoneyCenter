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

        // Year operations
        public async Task<List<Year>> GetYears()
        {
            await InitializeDatabase();
            return await _database.GetAllYearsAsync();
        }

        public async Task AddYear(int yearValue)
        {
            await InitializeDatabase();
            await _database.AddYearWithMonthsAsync(yearValue);
        }

        // Month operations
        public async Task<List<Month>> GetMonthsByYear(int yearId)
        {
            await InitializeDatabase();
            return await _database.GetMonthsByYearAsync(yearId);
        }

        public async Task<int> AssignBudgetToMonth(int monthId, string budgetId)
        {
            await InitializeDatabase();
            return await _database.AssignBudgetToMonthAsync(monthId, budgetId);
        }

        public async Task<Budget> GetBudgetForMonth(int monthId)
        {
            await InitializeDatabase();
            return await _database.GetBudgetForMonthAsync(monthId);
        }

        // Budget operations
        public async Task<List<Budget>> GetBudgets()
        {
            await InitializeDatabase();
            return await _database.GetAllBudgetsAsync();
        }

        // MasterCategory operations
        public async Task<List<MasterCategory>> GetMasterCategories()
        {
            await InitializeDatabase();
            return await _database.GetAllMasterCategoriesAsync();
        }

        public async Task<MasterCategory> GetMasterCategoryById(int categoryId)
        {
            await InitializeDatabase();
            return await _database.GetMasterCategoryByIdAsync(categoryId);
        }

        public async Task<List<MasterCategory>> GetMasterCategoriesByType(string type)
        {
            await InitializeDatabase();
            return await _database.GetMasterCategoriesByTypeAsync(type);
        }

        public async Task AddMasterCategory(MasterCategory category)
        {
            await InitializeDatabase();
            await _database.InsertMasterCategoryAsync(category);
        }

        public async Task UpdateMasterCategory(MasterCategory category)
        {
            await InitializeDatabase();
            await _database.UpdateMasterCategoryAsync(category);
        }

        public async Task DeleteMasterCategory(int categoryId)
        {
            await InitializeDatabase();
            var category = await _database.GetMasterCategoryByIdAsync(categoryId);
            if (category != null)
            {
                await _database.DeleteMasterCategoryAsync(category);
            }
        }

        // Budget-Category assignments
        public async Task<List<BudgetMasterCategory>> GetCategoriesByBudgetId(string budgetId)
        {
            await InitializeDatabase();
            return await _database.GetCategoriesByBudgetIdNewAsync(budgetId);
        }

        public async Task AssignCategoryToBudget(string budgetId, int masterCategoryId, decimal amount, bool isRecurring, int? dayOfMonth)
        {
            await InitializeDatabase();
            var assignment = new BudgetMasterCategory
            {
                BudgetId = budgetId,
                MasterCategoryId = masterCategoryId,
                Amount = amount,
                IsRecurring = isRecurring,
                DayOfMonth = dayOfMonth
            };
            await _database.InsertBudgetMasterCategoryAsync(assignment);
        }

        public async Task RemoveCategoryFromBudget(string budgetId, int masterCategoryId)
        {
            await InitializeDatabase();
            await _database.RemoveBudgetCategoryAssignmentAsync(budgetId, masterCategoryId);
        }

        public async Task UpdateBudgetCategoryAssignment(string budgetId, int masterCategoryId, decimal amount, bool isRecurring, int? dayOfMonth)
        {
            await InitializeDatabase();
            var assignment = await _database.GetBudgetCategoryAssignmentAsync(budgetId, masterCategoryId);
            if (assignment != null)
            {
                assignment.Amount = amount;
                assignment.IsRecurring = isRecurring;
                assignment.DayOfMonth = dayOfMonth;
                await _database.UpdateBudgetMasterCategoryAsync(assignment);
            }
        }

        // PaymentAccount operations
        public async Task<List<PaymentAccount>> GetPaymentAccounts()
        {
            await InitializeDatabase();
            return await _database.GetAllPaymentAccountsAsync();
        }

        public async Task AddPaymentAccount(PaymentAccount account)
        {
            await InitializeDatabase();
            await _database.InsertPaymentAccountAsync(account);
        }

        public async Task UpdatePaymentAccount(PaymentAccount account)
        {
            await InitializeDatabase();
            await _database.UpdatePaymentAccountAsync(account);
        }

        public async Task DeletePaymentAccount(int accountId)
        {
            await InitializeDatabase();
            var account = await _database.GetPaymentAccountByIdAsync(accountId);
            if (account != null)
            {
                await _database.DeletePaymentAccountAsync(account);
            }
        }

        // SavingsPod operations
        public async Task<List<SavingsPod>> GetAllSavingsPods()
        {
            await InitializeDatabase();
            return await _database.GetAllSavingsPodsAsync();
        }

        public async Task<SavingsPod> GetSavingsPodByMasterCategoryId(int masterCategoryId)
        {
            await InitializeDatabase();
            return await _database.GetSavingsPodByMasterCategoryIdAsync(masterCategoryId);
        }

        public async Task CreateSavingsPod(int masterCategoryId)
        {
            await InitializeDatabase();
            var category = await _database.GetMasterCategoryByIdAsync(masterCategoryId);
            if (category != null && category.Type == "savings")
            {
                var existingPod = await GetSavingsPodByMasterCategoryId(masterCategoryId);
                if (existingPod == null)
                {
                    var pod = new SavingsPod
                    {
                        MasterCategoryId = masterCategoryId,
                        Name = category.Name,
                        CurrentBalance = 0m,
                        CreatedDate = DateTime.Now
                    };
                    await _database.InsertSavingsPodAsync(pod);
                }
            }
        }

        public async Task<decimal> GetSavingsPodBalance(int savingsPodId)
        {
            await InitializeDatabase();
            return await _database.GetSavingsPodBalanceAsync(savingsPodId);
        }

        public async Task TransferExpenseToSavingsPod(string expenseId, int savingsPodId)
        {
            await InitializeDatabase();
            var expense = await _database.GetExpenseAsync(expenseId);
            if (expense != null)
            {
                expense.SavingsPodId = savingsPodId;
                await _database.UpdateExpenseAsync(expense);
                
                // Update the SavingsPod balance
                var pod = await _database.GetSavingsPodByIdAsync(savingsPodId);
                if (pod != null)
                {
                    pod.CurrentBalance = await _database.GetSavingsPodBalanceAsync(savingsPodId);
                    await _database.UpdateSavingsPodAsync(pod);
                }
            }
        }

        // Expense operations
        public async Task<List<Expense>> GetExpensesByMonthId(int monthId)
        {
            await InitializeDatabase();
            return await _database.GetExpensesByMonthIdAsync(monthId);
        }

        public async Task AddExpense(int monthId, Expense expense)
        {
            await InitializeDatabase();
            expense.MonthId = monthId;
            
            // If expense matches a savings category, auto-assign to savings pod
            var savingsPod = await _database.GetSavingsPodByMasterCategoryIdAsync(expense.MasterCategoryId);
            if (savingsPod != null)
            {
                expense.SavingsPodId = savingsPod.Id;
            }
            
            await _database.InsertExpenseAsync(expense);
        }

        public async Task UpdateExpense(int monthId, Expense expense)
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

        public async Task<Expense> GetExpense(string expenseId)
        {
            await InitializeDatabase();
            return await _database.GetExpenseAsync(expenseId);
        }

        // Recurring expense logic
        public async Task ProcessRecurringExpenses(int monthId)
        {
            await InitializeDatabase();
            // Get the month by ID from database
            var months = await _database.GetAllYearsAsync();
            Month month = null;
            foreach (var year in months)
            {
                var yearMonths = await _database.GetMonthsByYearAsync(year.Id);
                month = yearMonths.FirstOrDefault(m => m.Id == monthId);
                if (month != null) break;
            }

            if (month == null || string.IsNullOrEmpty(month.BudgetId))
                return;

            var recurringCategories = await _database.GetRecurringCategoriesForBudgetAsync(month.BudgetId);
            foreach (var categoryAssignment in recurringCategories)
            {
                if (categoryAssignment.IsRecurring && categoryAssignment.DayOfMonth.HasValue)
                {
                    var masterCategory = await _database.GetMasterCategoryByIdAsync(categoryAssignment.MasterCategoryId);
                    if (masterCategory != null)
                    {
                        var expenseDate = new DateTime(DateTime.Now.Year, month.MonthNumber, categoryAssignment.DayOfMonth.Value);
                        var expense = new Expense
                        {
                            Id = Guid.NewGuid().ToString(),
                            MonthId = monthId,
                            MasterCategoryId = masterCategory.Id,
                            Amount = categoryAssignment.Amount,
                            Date = expenseDate,
                            BudgetId = month.BudgetId,
                            Destination = $"Auto-recurring: {masterCategory.Name}",
                            Details = $"Auto-generated recurring expense for {masterCategory.Name}"
                        };
                        
                        // If it's a savings category, assign to savings pod
                        if (masterCategory.Type == "savings")
                        {
                            var savingsPod = await _database.GetSavingsPodByMasterCategoryIdAsync(masterCategory.Id);
                            if (savingsPod != null)
                            {
                                expense.SavingsPodId = savingsPod.Id;
                            }
                        }
                        
                        await _database.InsertExpenseAsync(expense);
                    }
                }
            }
        }

        // Initialization/Seeding
        public async Task SeedInitialData()
        {
            await InitializeDatabase();

            // Check if data already exists
            var budgetCount = await _database.GetBudgetCountAsync();
            if (budgetCount == 0)
            {
                // Create initial budgets
                var initialBudgets = GetInitialBudgets();
                await _database.InsertAllBudgetsAsync(initialBudgets);

                // Create master categories
                var masterCategories = GetInitialMasterCategories();
                await _database.InsertAllMasterCategoriesAsync(masterCategories);

                // Assign categories to budgets
                foreach (var budget in initialBudgets)
                {
                    foreach (var categoryAssignment in GetCategoryAssignmentsForBudget(budget, masterCategories))
                    {
                        categoryAssignment.BudgetId = budget.Id;
                        await _database.InsertBudgetMasterCategoryAsync(categoryAssignment);
                    }
                }

                // Create default payment accounts
                var defaultAccounts = GetDefaultPaymentAccounts();
                foreach (var account in defaultAccounts)
                {
                    var existing = await _database.GetPaymentAccountByNameAsync(account.Name);
                    if (existing == null)
                    {
                        await _database.InsertPaymentAccountAsync(account);
                    }
                }

                // Create savings pods for savings categories
                var savingsCategories = masterCategories.Where(c => c.Type == "savings");
                foreach (var category in savingsCategories)
                {
                    var pod = new SavingsPod
                    {
                        MasterCategoryId = category.Id,
                        Name = category.Name,
                        CurrentBalance = 0m,
                        CreatedDate = DateTime.Now
                    };
                    await _database.InsertSavingsPodAsync(pod);
                }
            }

            // Add current year with all 12 months if it doesn't exist
            int currentYear = DateTime.Now.Year;
            await _database.AddYearWithMonthsAsync(currentYear);

            // Assign default budget to current month if not already assigned
            var months = await _database.GetMonthsByYearAsync(
                (await _database.GetAllYearsAsync()).First(y => y.YearValue == currentYear).Id);
            var currentMonth = months.FirstOrDefault(m => m.MonthNumber == DateTime.Now.Month);
            if (currentMonth != null && string.IsNullOrEmpty(currentMonth.BudgetId))
            {
                var defaultBudget = await _database.GetAllBudgetsAsync();
                if (defaultBudget.Any())
                {
                    await _database.AssignBudgetToMonthAsync(currentMonth.Id, defaultBudget.First().Id);
                }
            }
        }

        private List<Budget> GetInitialBudgets()
        {
            return new List<Budget>
            {
                new Budget
                {
                    Id = Guid.NewGuid().ToString(),
                    Name = "Basic Budget",
                    IsDefault = true,
                    Description = "A simple budget for getting started"
                },
                new Budget
                {
                    Id = Guid.NewGuid().ToString(),
                    Name = "Frugal Budget",
                    IsDefault = false,
                    Description = "A budget focused on maximizing savings"
                }
            };
        }

        private List<MasterCategory> GetInitialMasterCategories()
        {
            return new List<MasterCategory>
            {
                new MasterCategory { Name = "Income", Type = "income", Description = "Regular income" },
                new MasterCategory { Name = "Housing", Type = "expense", Description = "Rent/Mortgage and utilities" },
                new MasterCategory { Name = "Food", Type = "expense", Description = "Groceries and dining" },
                new MasterCategory { Name = "Utilities", Type = "expense", Description = "Electric, water, gas" },
                new MasterCategory { Name = "Transportation", Type = "expense", Description = "Car, gas, public transit" },
                new MasterCategory { Name = "Entertainment", Type = "expense", Description = "Movies, hobbies, etc." },
                new MasterCategory { Name = "Emergency Fund", Type = "savings", Description = "Emergency savings" },
                new MasterCategory { Name = "Retirement", Type = "savings", Description = "Retirement savings" },
                new MasterCategory { Name = "Vacation", Type = "savings", Description = "Vacation fund" }
            };
        }

        private List<BudgetMasterCategory> GetCategoryAssignmentsForBudget(Budget budget, List<MasterCategory> categories)
        {
            if (budget == null) return new List<BudgetMasterCategory>();

            if (budget.Name == "Basic Budget")
            {
                return new List<BudgetMasterCategory>
                {
                    new BudgetMasterCategory { MasterCategoryId = categories.First(c => c.Name == "Income").Id, Amount = 5000, IsRecurring = true, DayOfMonth = 1 },
                    new BudgetMasterCategory { MasterCategoryId = categories.First(c => c.Name == "Housing").Id, Amount = 1500, IsRecurring = true, DayOfMonth = 1 },
                    new BudgetMasterCategory { MasterCategoryId = categories.First(c => c.Name == "Food").Id, Amount = 500, IsRecurring = false },
                    new BudgetMasterCategory { MasterCategoryId = categories.First(c => c.Name == "Utilities").Id, Amount = 300, IsRecurring = true, DayOfMonth = 15 },
                    new BudgetMasterCategory { MasterCategoryId = categories.First(c => c.Name == "Transportation").Id, Amount = 200, IsRecurring = false },
                    new BudgetMasterCategory { MasterCategoryId = categories.First(c => c.Name == "Entertainment").Id, Amount = 200, IsRecurring = false },
                    new BudgetMasterCategory { MasterCategoryId = categories.First(c => c.Name == "Emergency Fund").Id, Amount = 500, IsRecurring = true, DayOfMonth = 1 }
                };
            }
            else if (budget.Name == "Frugal Budget")
            {
                return new List<BudgetMasterCategory>
                {
                    new BudgetMasterCategory { MasterCategoryId = categories.First(c => c.Name == "Income").Id, Amount = 5000, IsRecurring = true, DayOfMonth = 1 },
                    new BudgetMasterCategory { MasterCategoryId = categories.First(c => c.Name == "Housing").Id, Amount = 1200, IsRecurring = true, DayOfMonth = 1 },
                    new BudgetMasterCategory { MasterCategoryId = categories.First(c => c.Name == "Food").Id, Amount = 300, IsRecurring = false },
                    new BudgetMasterCategory { MasterCategoryId = categories.First(c => c.Name == "Utilities").Id, Amount = 250, IsRecurring = true, DayOfMonth = 15 },
                    new BudgetMasterCategory { MasterCategoryId = categories.First(c => c.Name == "Transportation").Id, Amount = 150, IsRecurring = false },
                    new BudgetMasterCategory { MasterCategoryId = categories.First(c => c.Name == "Entertainment").Id, Amount = 100, IsRecurring = false },
                    new BudgetMasterCategory { MasterCategoryId = categories.First(c => c.Name == "Retirement").Id, Amount = 1000, IsRecurring = true, DayOfMonth = 1 }
                };
            }

            return new List<BudgetMasterCategory>();
        }

        private List<PaymentAccount> GetDefaultPaymentAccounts()
        {
            return new List<PaymentAccount>
            {
                new PaymentAccount { Name = "Cash", Type = "cash", Description = "Physical cash" },
                new PaymentAccount { Name = "Credit Card", Type = "credit_card", Description = "Primary credit card" },
                new PaymentAccount { Name = "Debit Card", Type = "debit_card", Description = "Primary debit card" },
                new PaymentAccount { Name = "Bank Transfer", Type = "bank_transfer", Description = "Direct bank transfer" },
                new PaymentAccount { Name = "Check", Type = "check", Description = "Paper check" }
            };
        }
    }
}