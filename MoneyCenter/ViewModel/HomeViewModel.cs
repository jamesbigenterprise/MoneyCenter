using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.ObjectModel;
using MoneyCenter.Views;

using CommunityToolkit.Mvvm.Input;
using Realms;
using MoneyCenter.Model;
using MongoDB.Bson;
using MoneyCenter.Schema;

//Current problem, cannot reference objects in the view from the view model
//the navigation service would be great if it worked.


namespace MoneyCenter.ViewModel
{
    public partial class HomeViewModel : ObservableObject
    {
        private MoneyCenter.Model.Model model = new();
        public HomeViewModel() 
        {
            
        }
        [ObservableProperty]
        private ObservableCollection<SingleEntryDisplayData> expenses = new();

        [RelayCommand]
        async Task Tap()
        {
            await Shell.Current.GoToAsync(nameof(NewEntryView));
        }

        [RelayCommand]
        async Task Delete(int Id) 
        {
            await model.DeleteSingleEntry(Id);
            await populateExpenses();
        }

        //this class is never disposed
        public async Task populateExpenses() 
        {
            List<SingleEntryDataModel> currententies = await model.GetAllEntries();
            expenses.Clear();
            foreach (SingleEntryDataModel entry in currententies) 
            {
                //translate from the database to this specific card view
                expenses.Add(new SingleEntryDisplayData 
                    { 
                        Amount = entry.Amount,
                        Date = entry.Date.DateTime.ToLongDateString(),
                        Category = entry.Category,
                        Paragraph = entry.Details,
                        Id = entry.Id
                    });
            }

        }

    }
}
