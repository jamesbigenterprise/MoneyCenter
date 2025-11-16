using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoneyCenter.ViewModel.Objects
{
    public class MonthlyData
    {
        public decimal Income { get; set; }
        public List<Expense> Expenses { get; set; } = new List<Expense>();
        public string? BudgetId { get; set; }
    }
}
