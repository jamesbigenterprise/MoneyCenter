using MoneyCenter.ViewModel;

namespace MoneyCenter.Views;

public partial class NewEntryView : ContentPage
{
	public NewEntryView(NewEntryViewModel viewModel)
	{
		InitializeComponent();
        BindingContext = viewModel;
    }

}