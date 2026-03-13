using System;

namespace MoneyCenter.ViewModel.Objects
{
    public class Month
    {
        public int Id { get; set; }
        public int YearId { get; set; }
        public string BudgetId { get; set; } = string.Empty;
        public int MonthNumber { get; set; }
        public string MonthName { get; set; } = string.Empty;
    }
}
