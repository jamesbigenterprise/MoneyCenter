using MoneyCenter.ViewModel;

namespace MoneyCenter.Views;

public partial class SavingsView : ContentView
{
    private bool _isDesktop;

    public bool IsDesktop
    {
        get => _isDesktop;
        set
        {
            if (_isDesktop == value)
                return;

            _isDesktop = value;
            OnPropertyChanged(nameof(IsDesktop));
            OnPropertyChanged(nameof(IsMobile));
        }
    }

    public bool IsMobile => !IsDesktop;

    public SavingsView(SavingsViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
        SizeChanged += (_, _) => IsDesktop = Width >= 900;
        IsDesktop = true;
    }
}
