using MoneyCenter.ViewModel;
namespace MoneyCenter;

public partial class HomeView : ContentPage
{
    public HomeView()
	{
		InitializeComponent();
		BindingContext = new HomeViewModel();
    }
}

