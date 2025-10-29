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
        public decimal Amount { get; set; }
        public DateTime Date { get; set; }
        public string Category { get; set; } = string.Empty;
        public string Destination { get; set; } = string.Empty;
        public string Details { get; set; } = string.Empty;
    }
}
