using System;

namespace MoneyCenter.ViewModel.Objects
{
    public class MasterCategory
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty; // "income", "expense", "savings"
        public string Description { get; set; } = string.Empty;
    }
}
