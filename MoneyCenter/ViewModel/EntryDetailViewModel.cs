using CommunityToolkit.Mvvm.ComponentModel;


namespace MoneyCenter.ViewModel
{
    public partial class EntryDetailViewModel : ObservableObject
    {
        [ObservableProperty]
        private SingleEntryDisplayData entry;
    }
}
