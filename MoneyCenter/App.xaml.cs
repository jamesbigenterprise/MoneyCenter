using CommunityToolkit.Mvvm.Messaging;
using MoneyCenter.Messages;

namespace MoneyCenter;

public partial class App : Application
{
    private readonly IServiceProvider _serviceProvider;

    public App(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
        InitializeComponent();
        UserAppTheme = AppTheme.Light;

        MainPage = _serviceProvider.GetRequiredService<AppShell>();

        WeakReferenceMessenger.Default.Register<NavigateToMainMessage>(this, (_, _) =>
        {
            MainThread.BeginInvokeOnMainThread(() =>
            {
                Windows[0].Page = _serviceProvider.GetRequiredService<MainView>();
            });
        });
    }
}
