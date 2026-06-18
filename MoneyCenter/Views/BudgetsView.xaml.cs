using MoneyCenter.ViewModel;

namespace MoneyCenter.Views;

public partial class BudgetsView : ContentView
{
    private bool _isDesktop;

    public bool IsDesktop
    {
        get => _isDesktop;
        set
        {
            if (_isDesktop != value)
            {
                _isDesktop = value;
                OnPropertyChanged(nameof(IsDesktop));
                OnPropertyChanged(nameof(IsMobile));
            }
        }
    }

    public bool IsMobile => !IsDesktop;

    public BudgetsView(BudgetsViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
        SizeChanged += OnSizeChanged;
        IsDesktop = true;
    }

    private void OnSizeChanged(object sender, EventArgs e)
    {
        IsDesktop = Width >= 900;
    }

    protected override void OnPropertyChanged(string propertyName = null)
    {
        base.OnPropertyChanged(propertyName);
        if (propertyName == nameof(IsVisible) && IsVisible && BindingContext is BudgetsViewModel vm)
        {
            _ = vm.LoadDataAsync();
        }
    }
}
