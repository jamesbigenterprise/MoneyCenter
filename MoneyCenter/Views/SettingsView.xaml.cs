using MoneyCenter.ViewModel;

namespace MoneyCenter.Views;


    public partial class SettingsView : ContentView
    {
        private bool _isDesktop = true;

        public bool IsDesktop
        {
            get => _isDesktop;
            set
            {
                if (_isDesktop == value)
                    return;

                _isDesktop = value;
                OnPropertyChanged(nameof(IsDesktop));
            }
        }

        public SettingsView(SettingsViewModel viewModel)
        {
            InitializeComponent();
            BindingContext = viewModel;
            SizeChanged += (_, _) => IsDesktop = Width >= 900;
        }
    }
