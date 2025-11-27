using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MoneyCenter.SQLiteWrapper;
using System.Threading.Tasks;

namespace MoneyCenter.ViewModel
{
    public partial class PermissionsViewModel : ObservableObject
    {
        private readonly MoneyCenterDatabase _database;

        public PermissionsViewModel(MoneyCenterDatabase database)
        {
            _database = database;
        }

        [RelayCommand]
        public async Task FinishSetup()
        {
            await _database.InitializeAsync();
            await Shell.Current.GoToAsync(nameof(MainView));
        }
    }
}
