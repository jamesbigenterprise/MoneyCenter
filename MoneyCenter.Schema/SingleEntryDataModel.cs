using SQLite;

namespace MoneyCenter.Schema
{
    public class SingleEntryDataModel
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        public DateTime Date { get; set; }
        public string Store { get; set; }
        public string Details { get; set; }
        public string Amount { get; set; }
        public string Category { get; set; }
        public string PmtMethod { get; set; }
        public string ApplyTo { get; set; }

    }
}
