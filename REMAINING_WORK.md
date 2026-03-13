# MoneyCenter - Remaining Work & Recommendations

## ?? Remaining Frontend Polish Items

### 1. **NewEntryView.xaml - Picker Implementation** 
   - **Current State:** Using numeric ID Entry fields as placeholder
   - **Recommended Approach:** 
     ```xaml
     <Picker ItemsSource="{Binding AvailableCategories}"
             ItemDisplayBinding="{Binding Name}"
             SelectedItem="{Binding SelectedCategory, Mode=TwoWay}"/>
     ```
   - **Action Items:**
     - Add ObservableCollections to NewEntryViewModel: `AvailableCategories`, `AvailableAccounts`, `AvailableMonths`
     - Load these collections in NewEntryViewModel constructor or in a LoadData() method
     - Update bindings in NewEntryView.xaml
     - Add validation before Save

### 2. **ExpensesView.xaml - Update Bindings**
   - **Current State:** Likely still using old string-based properties
   - **Recommended Approach:**
     - Update to use Year/Month hierarchies
     - Add Year picker first, then load Months
     - Display current month's budget name
   - **Action Items:**
     - Review ExpensesView.xaml bindings
     - Update to use SelectedYearId, SelectedMonthId instead of SelectedMonth string
     - Add DisplayAlert for better UX feedback

### 3. **BudgetsView.xaml - Category Display**
   - **Current State:** Needs update to show MasterCategories properly
   - **Recommended Approach:**
     - Display budget info with list of assigned categories
     - Show Amount, IsRecurring, DayOfMonth for each category
   - **Action Items:**
     - Update bindings to BudgetMasterCategory collections
     - Add ability to assign/remove categories from budget
     - Show category type and description

### 4. **Create SavingsView** (New)
   - **Purpose:** Display and manage savings pods
   - **Content:**
     - List all savings pods with current balance
     - Show expenses contributed to each pod
     - Visualize progress toward savings goals (optional)
   - **Action Items:**
     - Create SavingsView.xaml
     - Create/update SavingsViewModel
     - Wire to Shell navigation

### 5. **DashboardView.xaml - Update for Hierarchy**
   - **Current State:** Needs refactoring to use new model
   - **Recommended Approach:**
     - Show current month's budget and balance
     - Display recent expenses by category
     - Show savings pod summaries
   - **Action Items:**
     - Update bindings to use SelectedMonthId, RecentExpenses, etc.
     - Add charts/visualizations (optional, using OxyPlot or similar)

### 6. **Create PaymentAccountsView** (New)
   - **Purpose:** Manage payment accounts
   - **Content:**
     - List all payment accounts
     - Add/Edit/Delete accounts
   - **Action Items:**
     - Create PaymentAccountsView.xaml
     - Create PaymentAccountsViewModel with CRUD methods
     - Wire to Shell navigation

### 7. **Create MasterCategoriesView** (New)
   - **Purpose:** Manage master categories
   - **Content:**
     - List categories by type (income, expense, savings)
     - Add/Edit/Delete categories
     - View which budgets use each category
   - **Action Items:**
     - Create MasterCategoriesView.xaml
     - Create MasterCategoriesViewModel with CRUD methods
     - Wire to Shell navigation

---

## ?? Backend Enhancement Recommendations

### 1. **Data Validation**
   - Add validation for MasterCategoryId, PaymentAccountId before saving Expense
   - Validate BudgetId exists before assigning to Month
   - Validate DayOfMonth (1-31) for recurring categories

### 2. **Error Handling**
   - Add try-catch blocks to all database operations in Model layer
   - Implement proper error logging
   - Add user-friendly error messages in ViewModels

### 3. **Performance Optimization**
   - Add indexing for foreign keys (already have indexed: MonthId, MasterCategoryId, PaymentAccountId, SavingsPodId, BudgetId on Expense)
   - Consider caching MasterCategories and PaymentAccounts (rarely change)
   - Add pagination for large expense lists

### 4. **Reporting Methods** (Future Enhancement)
   ```csharp
   // Add to IModel interface
   Task<decimal> GetCategoryTotalByMonth(int masterCategoryId, int monthId);
   Task<Dictionary<int, decimal>> GetCategoryBreakdownByMonth(int monthId);
   Task<decimal> GetSavingsPodTotalByYear(int savingsPodId, int year);
   Task<List<Expense>> GetExpensesByCategory(int masterCategoryId, DateRange range);
   ```

### 5. **Audit Trail** (Optional)
   - Add CreatedDate, ModifiedDate, CreatedBy to Schema entities
   - Track changes to amounts and assignments
   - Useful for reconciliation and debugging

---

## ?? Testing Recommendations

### Unit Tests to Add:
1. **MoneyCenterModel Tests:**
   - `ProcessRecurringExpenses_CreatesCorrectEntries()`
   - `AddExpense_SavesEntryToSavingsPod_WhenCategoryIsSavingsType()`
   - `AssignBudgetToMonth_UpdatesCorrectMonth()`
   - `TransferExpenseToSavingsPod_UpdatesBalance()`

2. **ViewModel Tests:**
   - `ExpensesViewModel_LoadExpenses_BindsCorrectly()`
   - `NewEntryViewModel_AddExpense_CallsModelWithCorrectData()`

3. **Integration Tests:**
   - Full workflow: Create Budget ? Assign to Month ? Add Expenses ? Verify Savings Pods

### Manual Testing Checklist:
- [ ] Create new budget with recurring categories
- [ ] Assign budget to month
- [ ] Verify recurring expenses auto-populate on specified days
- [ ] Add expense matching savings category ? verify in savings pod
- [ ] Transfer expense to different savings pod ? verify balance updates
- [ ] Delete category ? verify it's removed from budget
- [ ] Add new year ? verify 12 months created

---

## ?? Database Migration (If Existing Data)

**If upgrading from old schema:**

```sql
-- Migration Steps:
1. Backup existing database
2. Create new tables (done by InitializeAsync)
3. Migrate old BudgetCategory data to MasterCategory:
   INSERT INTO MasterCategory (Name, Type, Description)
   SELECT DISTINCT Name, Type, '' FROM BudgetCategory
   WHERE Type IN ('income', 'expense', 'savings')
4. Migrate old BudgetCategory assignments to BudgetMasterCategory:
   INSERT INTO BudgetMasterCategory (BudgetId, MasterCategoryId, Amount, IsRecurring, DayOfMonth)
   SELECT bc.BudgetId, mc.Id, bc.Amount, bc.IsRecurring, bc.DayOfMonth
   FROM BudgetCategory bc
   JOIN MasterCategory mc ON bc.Name = mc.Name
5. Create PaymentAccount records from unique Expense.PaymentMethod values
6. Update Expense records to use FK IDs instead of strings
7. Drop old BudgetCategory table
```

---

## ?? UI/UX Improvements (Optional)

1. **Expense Entry Flow:**
   - Add confirmation dialog before saving
   - Show calculated total with running balance
   - Clear form after successful save with confirmation message

2. **Budget Overview:**
   - Show spending vs. budget percentage for each category
   - Color-code overspending (red) vs. under-budget (green)
   - Show month-to-month trends

3. **Savings Pod Visualization:**
   - Progress bars toward savings goals
   - Historical balance growth charts
   - Contribution frequency statistics

4. **Navigation:**
   - Add breadcrumb trail (Year > Month > Budget > Expenses)
   - Quick navigation shortcuts for prev/next month
   - Search expenses by date range or category

---

## ?? Documentation to Create

1. **User Manual:**
   - How to create budgets
   - Setting up recurring expenses
   - Managing savings pods
   - Viewing reports

2. **Developer Guide:**
   - Schema diagram
   - ViewModel architecture
   - Extension method usage
   - Database access patterns

3. **Architecture Decision Records (ADRs):**
   - Why separate MasterCategory from BudgetCategory
   - Why use SavingsPods as separate table
   - Recurring expense processing approach

---

## ?? Deployment Checklist

- [ ] All unit tests passing
- [ ] Integration tests passing
- [ ] Manual QA testing completed
- [ ] Database migration script tested (if applicable)
- [ ] Seed data generates correctly
- [ ] Views render without errors
- [ ] Performance acceptable (< 1s for typical operations)
- [ ] Error handling in place
- [ ] User documentation ready
- [ ] Release notes prepared

---

## ?? Priority Matrix

### High Priority (Do Next):
1. Picker controls in NewEntryView
2. SavingsView implementation
3. Complete backend validation
4. Unit tests for core logic

### Medium Priority (Do Soon):
1. Payment accounts management view
2. Master categories management view
3. Dashboard visualization improvements
4. Error handling enhancements

### Low Priority (Nice to Have):
1. Advanced reporting features
2. Audit trail implementation
3. Performance optimizations beyond current
4. Historical analytics/trends

---

## ?? Notes for Future Development

- **Framework:** Using .NET MAUI - keep in mind MAUI-specific features and limitations
- **MVVM Toolkit:** Using Community Toolkit's MVVM - understand partial method patterns and code generation
- **SQLite-net:** Async-based library - ensure all DB operations are async
- **Architecture:** Keep clear separation between Schema, ViewModel, and View layers using extension methods
- **Data Flow:** Year ? Month ? Expense hierarchy should be maintained in all operations

---

**Status:** Implementation Complete ?  
**Next Phase:** Frontend Polish & UI Enhancement  
**Estimated Time:** 4-6 hours for remaining UI items
