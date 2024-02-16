using CommunityToolkit.Mvvm.ComponentModel;
using MoneyCenter.Model;
using MoneyCenter.ViewModel.Messages;

namespace MoneyCenter.ViewModel
{
    public partial class NewEntryViewModel : ObservableObject
    {
        public event EventHandler CloseRequested;
        public event EventHandler SaveRequested;
        public Command CloseCommand { get; }
        public Command SaveCommand { get; }
        [ObservableProperty]
        private NewEntryModel newEntryModel = new();
        private MoneyCenter.Model.Model model = new();
        public NewEntryViewModel() 
        {
            CloseCommand = new Command(OnCloseCommand);
            SaveCommand = new Command(OnSaveCommand);
        }

        private void OnCloseCommand()
        {
            // Send a message indicating that the modal is closed
            MessagingCenter.Send(new ModalClosedMessage(), nameof(ModalClosedMessage));
            CloseRequested?.Invoke(this, EventArgs.Empty);
        }
        private void OnSaveCommand() 
        {
            //do nothing for now but this would call the model to save data to the database
            var isRequested = SaveRequested;
            //create an object compatible with the model
            saveEntry();
            //close after the save is done
            OnCloseCommand();
        }
        private bool saveEntry() 
        {
            //redundant check since it is initialized in the contructor, add a validate inputs method instead
            if(newEntryModel != null) 
            {
                MoneyCenter.Model.SingleEntry singleEntry = new MoneyCenter.Model.SingleEntry
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
