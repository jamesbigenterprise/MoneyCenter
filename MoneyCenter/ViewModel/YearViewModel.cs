using CommunityToolkit.Mvvm.ComponentModel;
using MoneyCenter.Model;
using MoneyCenter.ViewModel.Objects;
using System.Collections.ObjectModel;
using SchemaYear = MoneyCenter.Schema.Year;

namespace MoneyCenter.ViewModel
{
    public partial class YearViewModel : ObservableObject
    {
        private readonly IModel _model;
        private readonly ObservableCollection<Budget> _budgets;
        private readonly ObservableCollection<MasterCategory> _masterCategories;
        private readonly int _yearDbId;
        private readonly bool _expandCurrentMonth;

        public YearViewModel(
            SchemaYear year,
            IModel model,
            ObservableCollection<Budget> budgets,
            ObservableCollection<MasterCategory> masterCategories,
            bool expandCurrentMonth = false)
        {
            _yearDbId = year.Id;
            YearValue = year.YearValue;
            _model = model;
            _budgets = budgets;
            _masterCategories = masterCategories;
            _expandCurrentMonth = expandCurrentMonth;
        }

        [ObservableProperty] private int yearValue;
        [ObservableProperty] private bool isExpanded;
        [ObservableProperty] private ObservableCollection<MonthViewModel> months = new();

        partial void OnIsExpandedChanged(bool value)
        {
            if (value && !Months.Any())
                Task.Run(LoadMonthsAsync);
        }

        public async Task LoadMonthsAsync()
        {
            var schemaMonths = await _model.GetMonthsByYear(_yearDbId);
            MainThread.BeginInvokeOnMainThread(() =>
            {
                Months.Clear();
                foreach (var month in schemaMonths)
                {
                    var monthViewModel = new MonthViewModel(month, _model, _budgets, _masterCategories);
                    if (_expandCurrentMonth && month.MonthNumber == DateTime.Now.Month)
                        monthViewModel.IsExpanded = true;
                    Months.Add(monthViewModel);
                }
            });
        }

        [CommunityToolkit.Mvvm.Input.RelayCommand]
        private void ToggleExpand() => IsExpanded = !IsExpanded;
    }
}
