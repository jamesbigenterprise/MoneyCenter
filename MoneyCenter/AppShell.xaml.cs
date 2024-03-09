using MoneyCenter.Views;

namespace MoneyCenter;

public partial class AppShell : Shell
{
	public AppShell()
	{
		InitializeComponent();

		Routing.RegisterRoute(nameof(NewEntryView), typeof(NewEntryView));
	}
}
