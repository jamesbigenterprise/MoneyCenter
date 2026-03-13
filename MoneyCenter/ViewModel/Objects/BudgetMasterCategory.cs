using System;

namespace MoneyCenter.ViewModel.Objects
{
    public class BudgetMasterCategory
    {
        public int Id { get; set; }
        public string BudgetId { get; set; } = string.Empty;
        public int MasterCategoryId { get; set; }
        public decimal Amount { get; set; }
        public bool IsRecurring { get; set; }
        public int? DayOfMonth { get; set; }
    }
}
