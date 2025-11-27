using CommunityToolkit.Maui;
using CommunityToolkit.Maui.Core;
using CommunityToolkit.Maui.Markup;
using CommunityToolkit.Mvvm;
using Microsoft.Extensions.Logging;
using Microsoft.Maui.Controls.Compatibility.Hosting;
using MoneyCenter.Model;
using MoneyCenter.Services;
using MoneyCenter.SQLiteWrapper;
using MoneyCenter.ViewModel;
using MoneyCenter.Views;
using MoneyCenter.Views.Icons;

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
        // Core services
        builder.Services.AddSingleton<IModel, MoneyCenterModel>();
        builder.Services.AddSingleton<MoneyCenterDatabase>();

        // MainView and MainViewModel
        builder.Services.AddSingleton<MainView>();
        builder.Services.AddSingleton<MainViewModel>();
        builder.Services.AddSingleton<WelcomePage>();

        builder.Services.AddSingleton<WelcomeViewModel>();
        builder.Services.AddSingleton <PermissionsPage>();
        builder.Services.AddSingleton<PermissionsViewModel>();

        builder.Services.AddSingleton<IDeviceDisplay>(DeviceDisplay.Current);

        builder.Services.AddTransient<NewEntryView>();
        builder.Services.AddTransient<NewEntryViewModel>();

        builder.Services.AddSingleton<IFinancialService, FinancialService>();
        builder.Services.AddSingleton<IToastService, ToastService>();

        builder.Services.AddTransient<DashboardViewModel>();
        builder.Services.AddTransient<DashboardView>();

        builder.Services.AddTransient<ExpensesViewModel>();
        builder.Services.AddTransient<ExpensesView>();

        builder.Services.AddTransient<SavingsViewModel>();
        builder.Services.AddTransient<SavingsView>();

        builder.Services.AddTransient<BudgetsViewModel>();
        builder.Services.AddTransient<BudgetsView>();

        builder.Services.AddTransient<SettingsViewModel>();
        builder.Services.AddTransient<SettingsView>();

        return builder.Build();
    }
}