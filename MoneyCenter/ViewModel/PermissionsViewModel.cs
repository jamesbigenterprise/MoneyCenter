using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using MoneyCenter.Messages;
using MoneyCenter.Model;


namespace MoneyCenter.ViewModel
{
    public partial class PermissionsViewModel : ObservableObject
    {
        private readonly IModel _model;

        public PermissionsViewModel(IModel model)
        {
            _model = model;
        }

        [RelayCommand]
        public async Task FinishSetup()
        {
            await _model.InitializeDatabase();
            await _model.SeedInitialData();
            WeakReferenceMessenger.Default.Send(new NavigateToMainMessage());
        }
    }
}