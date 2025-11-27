using MoneyCenter.SQLiteWrapper;
using MoneyCenter.ViewModel;

namespace MoneyCenter.Views;

public partial class PermissionsPage : ContentPage
{
    public PermissionsPage(PermissionsViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
}