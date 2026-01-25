using SQLite;
using System;
using System.Collections.Generic;
using System.Text;

namespace MoneyCenter.Schema
{
    public class BudgetCategory
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        public string BudgetId { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public string Type { get; set; } = "expense";
        public bool IsRecurring { get; set; }
        public int? DayOfMonth { get; set; }
    }
}
