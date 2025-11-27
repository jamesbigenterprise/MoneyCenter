using MoneyCenter.Views;

namespace MoneyCenter;

public partial class AppShell : Shell
{
	public AppShell()
	{
		InitializeComponent();
        Routing.RegisterRoute(nameof(WelcomePage), typeof(WelcomePage));
        Routing.RegisterRoute(nameof(MainView), typeof(MainView));
        Routing.RegisterRoute(nameof(NewEntryView), typeof(NewEntryView));
        Routing.RegisterRoute(nameof(DashboardView), typeof(DashboardView));
        Routing.RegisterRoute(nameof(ExpensesView), typeof(ExpensesView));
        Routing.RegisterRoute(nameof(SavingsView), typeof(SavingsView));
        Routing.RegisterRoute(nameof(BudgetsView), typeof(BudgetsView));
        Routing.RegisterRoute(nameof(SettingsView), typeof(SettingsView));
        Routing.RegisterRoute(nameof(PermissionsPage), typeof(PermissionsPage));

    }
}
