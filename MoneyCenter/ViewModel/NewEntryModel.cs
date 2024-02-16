using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoneyCenter.ViewModel
{
    public class NewEntryModel
    {
        public DateTime Date { get; set; }
        public string Store { get; set; }
        public string Details { get; set; }
        public string Amount { get; set; }
        public string Category { get; set; }
        public string PaymentMethod { get; set; }
        public string ApplyTo { get; set; }
    }
}
