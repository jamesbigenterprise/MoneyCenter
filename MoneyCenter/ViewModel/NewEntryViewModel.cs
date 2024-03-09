using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MoneyCenter.Model;

namespace MoneyCenter.ViewModel
{
    public partial class NewEntryViewModel : ObservableObject
    {


        [ObservableProperty]
        private NewEntryInputData newEntryModel = new();
        private MoneyCenter.Model.Model model = new();
        private HomeViewModel _home;
        public NewEntryViewModel(HomeViewModel vm) 
        {
            _home = vm;

        }

        [RelayCommand]
        async Task Save()
        {
            saveEntry();
            await Close();
        }

        [RelayCommand]
        async Task Close()
        {
            await Shell.Current.GoToAsync("..");
        }
        private bool saveEntry() 
        {
            //redundant check since it is initialized in the contructor, add a validate inputs method instead
            if(newEntryModel != null) 
            {
                MoneyCenter.Model.SingleEntryDataModel singleEntry = new MoneyCenter.Model.SingleEntryDataModel
                {
                    Date = newEntryModel.Date,
                    Store = newEntryModel.Store,
                    Details = newEntryModel.Details,
                    Amount = newEntryModel.Amount.ToString(),
                    Category = newEntryModel.Category,
                    PmtMethod = newEntryModel.PaymentMethod,
                    ApplyTo = newEntryModel.ApplyTo.ToString()
                };
                return model.AddEntry(singleEntry);
            }
            else { return false; }
        }
    }
}
