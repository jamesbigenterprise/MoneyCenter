using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MoneyCenter.Model;
using System.Threading.Tasks;

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
            await Shell.Current.GoToAsync(nameof(MainView));
        }
    }
}