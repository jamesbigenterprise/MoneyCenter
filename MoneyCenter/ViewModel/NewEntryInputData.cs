namespace MoneyCenter.ViewModel
{
    public class NewEntryInputData
    {
        public NewEntryInputData() 
        {
            Date = DateTime.Now;
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
