using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MoneyCenter.Schema;
using MoneyCenter.Model;

namespace MoneyCenter.ViewModel
{
    public partial class NewEntryViewModel : ObservableObject
    {


        [ObservableProperty]
        private NewEntryInputData newEntryModel = new();
        private readonly IModel model;
        private MainViewModel _home;
        public NewEntryViewModel(MainViewModel vm, IModel model) 
        {

            _home = vm;
            this.model = model;

        }

        [RelayCommand]
        async Task Save()
        {
            await saveEntry();
            await Close();
        }

        [RelayCommand]
        async Task Close()
        {
            await Shell.Current.GoToAsync("..");
        }
        private async Task saveEntry() 
        {
            Expense singleEntry = new Expense
            {
                Id = Guid.NewGuid().ToString(),
                Date = newEntryModel.Date,
                Destination = newEntryModel.Store,
                MasterCategoryId = newEntryModel.MasterCategoryId,
                Amount = newEntryModel.Amount,
                PaymentAccountId = newEntryModel.PaymentAccountId,
                Details = newEntryModel.Details,
                MonthId = newEntryModel.MonthId
            };
            
        }
    }
}
