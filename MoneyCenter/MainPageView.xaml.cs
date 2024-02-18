using MoneyCenter.ViewModel;
using MoneyCenter.Views.Modals;
namespace MoneyCenter;

public partial class MainPageView : ContentPage
{
    public MainPageView()
	{
		InitializeComponent();
		BindingContext = new HomeViewModel(Navigation);
    }
}

