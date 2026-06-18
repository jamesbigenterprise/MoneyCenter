using System;
using System.Collections.Generic;
using System.Windows.Input;
using CommunityToolkit.Mvvm.ComponentModel;

namespace MoneyCenter.ViewModel.Objects
{
    public partial class BudgetCategory : ObservableObject
    {
        public int Id { get; set; } // MasterCategoryId

        [ObservableProperty]
        private string name = string.Empty;

        [ObservableProperty]
        private decimal? amount;

        public decimal CurrentBalance { get; set; }

        [ObservableProperty]
        private string type = "expense";

        [ObservableProperty]
        private bool isRecurring;

        [ObservableProperty]
        private int? dayOfMonth;

        [ObservableProperty]
        private bool isEditing;

        public List<string> TypeOptions { get; set; } = new List<string>();
        public ICommand? EditCommand { get; set; }
        public ICommand? SaveCommand { get; set; }
        public ICommand? CancelCommand { get; set; }
        public ICommand? DeleteCommand { get; set; }

        public bool IsHidden { get; set; }
        public double Progress => !Amount.HasValue || Amount <= 0
            ? 0
            : Math.Min(1, (double)(CurrentBalance / Amount.Value));
        public string ProgressText => !Amount.HasValue || Amount <= 0
            ? "0% complete"
            : $"{Math.Round(Progress * 100)}% complete";
        public string FrequencyText => IsRecurring
            ? $"Recurring (Day {DayOfMonth ?? 1})"
            : "Flexible";

        partial void OnIsRecurringChanged(bool value)
        {
            if (!value)
            {
                DayOfMonth = null;
            }

            OnPropertyChanged(nameof(FrequencyText));
        }

        partial void OnDayOfMonthChanged(int? value)
        {
            OnPropertyChanged(nameof(FrequencyText));
        }
    }
}
