using CommunityToolkit.Mvvm.ComponentModel;

using System.Collections.ObjectModel;
using System.Windows.Input;
using Xamarin.Forms;
using MoneyCenter.NavigationService;

//Current problem, cannot reference objects in the view from the view model
    //the navigation service would be great if it worked.


namespace MoneyCenter.ViewModels
{
    public partial class HomeViewModel : ObservableObject
    {

        private readonly NavigationService _navigationService;
        public HomeViewModel() 
        {
            
        }
        [ObservableProperty]
        private ObservableCollection<SingleEntry> expenses = new();



    }
}
