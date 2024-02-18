using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.ObjectModel;
using MoneyCenter.Views.Modals;
using MoneyCenter.ViewModel.Messages;

//Current problem, cannot reference objects in the view from the view model
//the navigation service would be great if it worked.


namespace MoneyCenter.ViewModel
{
    public partial class HomeViewModel : ObservableObject
    {
        private readonly INavigation _navigation;
        private MoneyCenter.Model.Model model = new();
        public Command OpenModalCommand { get; }
        public HomeViewModel(INavigation navigation) 
        {
            _navigation = navigation ?? throw new ArgumentNullException(nameof(navigation));
            OpenModalCommand = new Command(ShowNewEntryModal);
            // Subscribe to the ModalClosedMessage
            MessagingCenter.Subscribe<ModalClosedMessage>(this, nameof(ModalClosedMessage), OnModalClosed);
            populateExpenses();
        }
        [ObservableProperty]
        private ObservableCollection<SingleEntryDisplayData> expenses = new();

        private void ShowNewEntryModal()
        {
            // Create a new instance of NewEntryPage
            var newEntryPage = new NewEntry();

            // Display it as a modal
            _navigation.PushModalAsync(newEntryPage);
        }
        private void OnModalClosed(ModalClosedMessage message)
        {
            // Call populateExpenses when the modal is closed
            populateExpenses();
        }

        private void populateExpenses() 
        {
            List<MoneyCenter.Model.SingleEntryDataModel> currententies = model.GetAllEntries();
            expenses.Clear();
            foreach (MoneyCenter.Model.SingleEntryDataModel entry in currententies) 
            {
                //translate from the database to this specific card view
                expenses.Add(new SingleEntryDisplayData 
                    { 
                        Amount = entry.Amount,
                        Date = entry.Date.DateTime.ToLongDateString(),
                        Category = entry.Category,
                        Paragraph = entry.Details
                    });
            }
        }

    }
}
