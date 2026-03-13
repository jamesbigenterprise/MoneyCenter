using SQLite;

namespace MoneyCenter.Schema
{
    public class Year
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        public int YearValue { get; set; }
    }
}
