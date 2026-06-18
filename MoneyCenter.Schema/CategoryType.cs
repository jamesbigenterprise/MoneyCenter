using SQLite;

namespace MoneyCenter.Schema
{
    public class CategoryType
    {
        [PrimaryKey]
        public string Key { get; set; } = string.Empty;

        [Unique]
        public string Name { get; set; } = string.Empty;

        public string Color { get; set; } = string.Empty;

        public string TextColor { get; set; } = string.Empty;

        public bool IsSystem { get; set; }
    }
}
