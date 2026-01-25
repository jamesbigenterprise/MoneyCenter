using SQLite;
using System;
using System.Collections.Generic;
using System.Text;

namespace MoneyCenter.Schema
{
    public class MonthlyData
    {
        [PrimaryKey]
        public string Month { get; set; } = string.Empty;
        public decimal Income { get; set; }
        public string? BudgetId { get; set; }
    }
}
