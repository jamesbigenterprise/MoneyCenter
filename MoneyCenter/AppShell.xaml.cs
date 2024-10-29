using MoneyCenter.Views;

namespace MoneyCenter;

public partial class AppShell : Shell
{
	public AppShell()
	{
		InitializeComponent();

		Routing.RegisterRoute(nameof(NewEntryView), typeof(NewEntryView));
		Routing.RegisterRoute(nameof(NewPage1), typeof(NewPage1));
	}
}
