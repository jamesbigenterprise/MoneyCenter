using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoneyCenter.ViewModel.Objects
{
    public class BudgetCategory
    {
        public string Name { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public string Type { get; set; } = "expense"; // will be its own type
        public bool IsRecurring { get; set; }
        public int? DayOfMonth { get; set; }
    }
}
