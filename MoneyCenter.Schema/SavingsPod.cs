using SQLite;
using System;

namespace MoneyCenter.Schema
{
    public class SavingsPod
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        
        [Unique]
        public int MasterCategoryId { get; set; } // FK to MasterCategory.Id (must be type "savings")
        
        public string Name { get; set; } = string.Empty; // Copied from MasterCategory for convenience
        
        public decimal CurrentBalance { get; set; } = 0m;
        
        public DateTime CreatedDate { get; set; } = DateTime.Now;
    }
}
