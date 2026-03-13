using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoneyCenter.ViewModel.Objects
{
    public class Expense
    {
        public string Id { get; set; } = string.Empty;
        public int MonthId { get; set; }
        public decimal Amount { get; set; }
        public DateTime Date { get; set; }
        public int MasterCategoryId { get; set; }
        public string Destination { get; set; } = string.Empty;
        public string Details { get; set; } = string.Empty;
        public int PaymentAccountId { get; set; }
        public string BudgetId { get; set; } = string.Empty;
        public int? SavingsPodId { get; set; }
    }
}
