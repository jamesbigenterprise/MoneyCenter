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
            //have the month be part of the input? yes
            string month = DateTime.Now.Month.ToString();
            //the object has a date but it will apply to whatever month the user is
            //editing since they can have late or early entries in the month

            // Add input validation here if needed


            Expense singleEntry = new Expense
            {
                Date = newEntryModel.Date,
                Destination = newEntryModel.Store,
                Category = newEntryModel.Category, //ensure the frontend category follows the db
                Amount = newEntryModel.Amount,
                PaymentMethod = newEntryModel.PaymentMethod,
                Details = newEntryModel.Details
            };
                await model.AddExpense(month, singleEntry);

        }
    }
}
