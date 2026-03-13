using SQLite;
using System;

namespace MoneyCenter.Schema
{
    public class MasterCategory
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        
        [Unique]
        public string Name { get; set; } = string.Empty;
        
        public string Type { get; set; } = string.Empty; // "income", "expense", "savings"
        
        public string Description { get; set; } = string.Empty;
    }
}
