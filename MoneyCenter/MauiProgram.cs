using Microsoft.Extensions.Logging;
using MoneyCenter.Model;
using MoneyCenter.ViewModel;
using MoneyCenter.Views;

namespace MoneyCenter;

public static class MauiProgram
{
	public static MauiApp CreateMauiApp()
	{
		var builder = MauiApp.CreateBuilder();
		builder
			.UseMauiApp<App>()
			.ConfigureFonts(fonts =>
			{
				fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
				fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
			});

#if DEBUG
        builder.Logging.AddDebug();
#endif
		builder.Services.AddSingleton<HomeView>();
        builder.Services.AddSingleton<HomeViewModel>();
		builder.Services.AddSingleton <IModel>(new MoneyCenterModel());

        builder.Services.AddTransient<NewEntryView>();
        builder.Services.AddTransient<NewEntryViewModel>();

        return builder.Build();
	}
}
