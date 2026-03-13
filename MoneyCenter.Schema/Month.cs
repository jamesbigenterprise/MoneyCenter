using SQLite;
using System;

namespace MoneyCenter.Schema
{
    public class Month
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        [Indexed]
        public int YearId { get; set; } // FK to Year.Id
        [Indexed]
        public string BudgetId { get; set; } = string.Empty; // FK to Budget.Id - a month has one assigned budget
        public int MonthNumber { get; set; } // 1-12
        public string MonthName { get; set; } = string.Empty; // e.g. "January"
    }
}
