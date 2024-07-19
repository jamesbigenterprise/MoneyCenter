using MoneyCenter.ViewModel;
namespace MoneyCenter;

public partial class HomeView : ContentPage
{
    public HomeView(HomeViewModel vm)
	{
		InitializeComponent();

		BindingContext = vm;
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

