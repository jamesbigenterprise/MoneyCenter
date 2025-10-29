using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoneyCenter.ViewModel.Objects
{
    public class Budget
    {
        public string Id { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public List<BudgetCategory> Categories { get; set; } = new List<BudgetCategory>();
        public bool IsDefault { get; set; }
        public string Description { get; set; } = string.Empty;
    }
}
