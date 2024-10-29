﻿using Microsoft.Extensions.Logging;
using MoneyCenter.Model;
using MoneyCenter.ViewModel;
using MoneyCenter.Views;
using CommunityToolkit.Mvvm;
using MoneyCenter.Views.Icons;
using CommunityToolkit.Maui;
using CommunityToolkit.Maui.Core;
using CommunityToolkit.Maui.Markup;
using Microsoft.Maui.Controls.Compatibility.Hosting;
namespace MoneyCenter;

public static class MauiProgram
{
	public static MauiApp CreateMauiApp()
	{
		var builder = MauiApp.CreateBuilder();
		builder
			.UseMauiApp<App>()
            .UseMauiCommunityToolkit()
			.UseMauiCommunityToolkitCore()
			.UseMauiCommunityToolkitMarkup()
			.UseMauiCompatibility()
            .ConfigureFonts(fonts =>
			{
				fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
				fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
				fonts.AddFont("MaterialIcons-Regular.ttf", nameof(MaterialIcon));
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
