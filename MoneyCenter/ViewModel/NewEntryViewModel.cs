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
            // Add input validation here if needed
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
