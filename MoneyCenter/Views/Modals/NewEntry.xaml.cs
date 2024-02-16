using MoneyCenter.ViewModel;

namespace MoneyCenter.Views.Modals;

public partial class NewEntry : ContentPage
{
	public NewEntry()
	{
		InitializeComponent();
        var viewModel = new NewEntryViewModel();
        viewModel.CloseRequested += OnCloseRequested;
        BindingContext = viewModel;
    }

    private async void OnCloseRequested(object sender, EventArgs e)
    {
        if (Navigation.ModalStack.Count > 0)
            await Navigation.PopModalAsync();
    }
}