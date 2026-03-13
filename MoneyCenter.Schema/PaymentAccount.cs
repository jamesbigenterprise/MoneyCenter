using SQLite;
using System;

namespace MoneyCenter.Schema
{
    public class PaymentAccount
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        
        [Unique]
        public string Name { get; set; } = string.Empty;
        
        public string Type { get; set; } = string.Empty; // "cash", "credit_card", "bank_transfer", "debit_card", etc.
        
        public string Description { get; set; } = string.Empty;
    }
}
