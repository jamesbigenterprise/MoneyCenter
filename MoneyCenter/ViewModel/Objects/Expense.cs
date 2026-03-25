using CommunityToolkit.Mvvm.Input;
using System;
using System.Windows.Input;

namespace MoneyCenter.ViewModel.Objects
{
    public class Expense
    {
        public string Id { get; set; } = string.Empty;
        public int MonthId { get; set; }
        public decimal Amount { get; set; }
        public DateTime Date { get; set; }
        public int MasterCategoryId { get; set; }
        public string CategoryName { get; set; } = string.Empty;
        public string Destination { get; set; } = string.Empty;
        public string Details { get; set; } = string.Empty;
        public int PaymentAccountId { get; set; }
        public string BudgetId { get; set; } = string.Empty;
        public int? SavingsPodId { get; set; }
        public ICommand DeleteCommand { get; set; }
    }
}
