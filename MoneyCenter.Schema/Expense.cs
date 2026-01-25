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
        public string MonthId { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public DateTime Date { get; set; }
        public string Category { get; set; } = string.Empty;
        public string Destination { get; set; } = string.Empty;
        public string Details { get; set; } = string.Empty;
    }
}
