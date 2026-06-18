namespace MoneyCenter.ViewModel
{
    public class NewEntryInputData
    {
        public NewEntryInputData() 
        {
            Date = DateTime.Now;
            Store = string.Empty;
            Details = string.Empty;
        }
        public DateTime Date { get; set; }
        public string Store { get; set; }
        public string Details { get; set; }
        public decimal Amount { get; set; }
        public int MasterCategoryId { get; set; }
        public int PaymentAccountId { get; set; }
        public int MonthId { get; set; }
    }
}
