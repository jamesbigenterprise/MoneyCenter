using MoneyCenter.ViewModel;
namespace MoneyCenter;

public partial class HomeView : ContentPage
{
    public HomeView()
	{
		InitializeComponent();
		BindingContext = new HomeViewModel();
    }

	protected override void OnAppearing() 
	{
		base.OnAppearing();
		if(BindingContext is HomeViewModel vm) 
		{
			Task.Run(async () => await vm.populateExpenses());
		}
	}
}

