using SQLite;
using System;

namespace MoneyCenter.Schema
{
    [Table("BudgetMasterCategory")]
    public class BudgetMasterCategory
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        
        [Indexed]
        public string BudgetId { get; set; } = string.Empty; // FK to Budget.Id
        
        [Indexed]
        public int MasterCategoryId { get; set; } // FK to MasterCategory.Id
        
        public decimal Amount { get; set; }
        
        public bool IsRecurring { get; set; }
        
        public int? DayOfMonth { get; set; } // Day of month for recurring expenses (1-31)
    }
}
