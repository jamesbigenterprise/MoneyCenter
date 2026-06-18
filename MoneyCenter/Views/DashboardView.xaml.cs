using MoneyCenter.ViewModel;
namespace MoneyCenter;

public partial class DashboardView : ContentView
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
            OnPropertyChanged(nameof(IsMobile));
        }
    }

    public bool IsMobile => !IsDesktop;

    public DashboardView(DashboardViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
        SizeChanged += (_, _) => IsDesktop = Width >= 900;
    }
}
