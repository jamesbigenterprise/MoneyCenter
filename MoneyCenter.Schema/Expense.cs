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
        public string Category { get; set; } = string.Empty;
        public string Destination { get; set; } = string.Empty;
        public string Details { get; set; } = string.Empty;
        //Have the payment method be the account so we can display expenses by account
        //Needs to be a separate table 
        public string PaymentMethod { get; set; }
        public string BudgetId { get; set; } // New: BudgetId for expense
    }
}
