using MoneyCenter.ViewModel;
using MoneyCenter.Views.Modals;
namespace MoneyCenter;

public partial class MainPage : ContentPage
{
    public MainPage()
	{
		InitializeComponent();
		BindingContext = new HomeViewModel(Navigation);
    }
}

