using MoneyCenter.ViewModel;

namespace MoneyCenter.Views;


    public partial class SettingsView : ContentView
    {
        public SettingsView(SettingsViewModel viewModel)
        {
            InitializeComponent();
            BindingContext = viewModel;
        }
    }