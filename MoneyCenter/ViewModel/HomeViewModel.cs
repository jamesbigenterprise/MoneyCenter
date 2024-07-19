using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.ObjectModel;
using MoneyCenter.Views;

using CommunityToolkit.Mvvm.Input;
using MoneyCenter.Schema;
using MoneyCenter.Model;

//Current problem, cannot reference objects in the view from the view model
//the navigation service would be great if it worked.


namespace MoneyCenter.ViewModel
{
    public partial class HomeViewModel : ObservableObject
    {
        private readonly IModel _model;

        public HomeViewModel(IModel model) 
        {
            _model = model;
            
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
            await _model.DeleteSingleEntry(Id);
            await populateExpenses();
        }

        //this class is never disposed
        public async Task populateExpenses() 
        {
            List<SingleEntryDataModel> currententies = await _model.GetAllEntries();
            expenses.Clear();
            foreach (SingleEntryDataModel entry in currententies) 
            {
                //translate from the database to this specific card view
                expenses.Add(new SingleEntryDisplayData 
                    { 
                        Amount = entry.Amount,
                        Date = entry.Date.ToShortDateString(),
                        Category = entry.Category,
                        Paragraph = entry.Details,
                        Id = entry.Id
                    });
            }

        }

    }
}
