using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MoneyCenter.Schema;

namespace MoneyCenter.ViewModel
{
    public partial class NewEntryViewModel : ObservableObject
    {


        [ObservableProperty]
        private NewEntryInputData newEntryModel = new();
        private MoneyCenter.Model.MoneyCenterModel model = new();
        private MainViewModel _home;
        public NewEntryViewModel(MainViewModel vm) 
        {

            _home = vm;

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
            //redundant check since it is initialized in the contructor, add a validate inputs method instead

                SingleEntryDataModel singleEntry = new SingleEntryDataModel
                {
                    Date = newEntryModel.Date,
                    Store = newEntryModel.Store,
                    Details = newEntryModel.Details,
                    Amount = newEntryModel.Amount.ToString(),
                    Category = newEntryModel.Category,
                    PmtMethod = newEntryModel.PaymentMethod,
                    ApplyTo = newEntryModel.ApplyTo.ToString()
                };
                await model.AddEntry(singleEntry);

        }
    }
}
