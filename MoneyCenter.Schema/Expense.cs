using SQLite;
using System;
using System.Collections.Generic;
using System.Text;

namespace MoneyCenter.Schema
{
    public class Expense
    {
        [PrimaryKey]
        public string Id { get; set; } = string.Empty;
        [Indexed] // [ForeignKey(typeof(Month))] 
        public int MonthId { get; set; } // FK to Month.Id, required
        public decimal Amount { get; set; }
        public DateTime Date { get; set; }
        [Indexed]
        public int MasterCategoryId { get; set; } // FK to MasterCategory.Id
        public string Destination { get; set; } = string.Empty;
        public string Details { get; set; } = string.Empty;
        [Indexed]
        public int PaymentAccountId { get; set; } // FK to PaymentAccount.Id
        public string BudgetId { get; set; } = string.Empty; // FK to Budget.Id
        [Indexed]
        public int? SavingsPodId { get; set; } // FK to SavingsPod.Id (for savings-type expenses)
    }
}
