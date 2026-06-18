using MoneyCenter.ViewModel;

namespace MoneyCenter.Views;

public partial class ExpensesView : ContentView
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

    public ExpensesView(ExpensesViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
        SizeChanged += OnSizeChanged;
        
        // Initialize to safe defaults based on assumed window size, will fix upon layout
        IsDesktop = true;
    }

    private void OnSizeChanged(object sender, EventArgs e)
    {
        // 600px breakpoint for mobile view
        IsDesktop = this.Width > 600;
    }

    protected override void OnPropertyChanged(string propertyName = null)
    {
        base.OnPropertyChanged(propertyName);
        if (propertyName == nameof(IsVisible) && IsVisible && BindingContext is ExpensesViewModel vm)
            vm.LoadData();
    }
}