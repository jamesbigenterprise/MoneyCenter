using CommunityToolkit.Maui;
using CommunityToolkit.Maui.Core;
using CommunityToolkit.Maui.Markup;
using CommunityToolkit.Mvvm;
using Microsoft.Extensions.Logging;
using Microsoft.Maui.Controls.Compatibility.Hosting;
using Microsoft.Maui.Handlers;
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
            .ConfigureMauiHandlers(handlers =>
            {
                EntryHandler.Mapper.AppendToMapping("MoneyCenterBorderlessEntry", (handler, view) =>
                {
#if WINDOWS
                    if (handler.PlatformView == null)
                    {
                        return;
                    }

                    TryApplyBorderlessTextBox(handler.PlatformView);
                    handler.PlatformView.Loaded += (_, _) => TryApplyBorderlessTextBox(handler.PlatformView);
                    handler.PlatformView.GotFocus += (_, _) => TryApplyBorderlessTextBox(handler.PlatformView);
#elif ANDROID
                    if (handler.PlatformView == null)
                    {
                        return;
                    }

                    handler.PlatformView.Background = null;
                    handler.PlatformView.SetPadding(0, 0, 0, 0);
#endif
                });

                PickerHandler.Mapper.AppendToMapping("MoneyCenterBorderlessPicker", (handler, view) =>
                {
#if WINDOWS
                    if (handler.PlatformView == null)
                    {
                        return;
                    }

                    TryApplyBorderlessComboBox(handler.PlatformView);
                    handler.PlatformView.Loaded += (_, _) => TryApplyBorderlessComboBox(handler.PlatformView);
                    handler.PlatformView.GotFocus += (_, _) => TryApplyBorderlessComboBox(handler.PlatformView);
#elif ANDROID
                    if (handler.PlatformView == null)
                    {
                        return;
                    }

                    handler.PlatformView.Background = null;
                    handler.PlatformView.SetPadding(0, 0, 0, 0);
#endif
                });
            })
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

        builder.Services.AddSingleton<AppShell>();

        // MainView and MainViewModel
        builder.Services.AddTransient<MainView>();
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
        builder.Services.AddSingleton<IConfirmationService, ConfirmationService>();

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

#if WINDOWS
    private static void TryApplyBorderlessTextBox(Microsoft.UI.Xaml.Controls.TextBox textBox)
    {
        try
        {
            ApplyBorderlessTextBox(textBox);
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"TextBox styling error: {ex.Message}");
        }
    }

    private static void ApplyBorderlessTextBox(Microsoft.UI.Xaml.Controls.TextBox textBox)
    {
        var transparent = new Microsoft.UI.Xaml.Media.SolidColorBrush(Microsoft.UI.Colors.Transparent);
        textBox.BorderThickness = new Microsoft.UI.Xaml.Thickness(0);
        textBox.Padding = new Microsoft.UI.Xaml.Thickness(0);
        textBox.Background = transparent;
        textBox.BorderBrush = transparent;
        textBox.UseSystemFocusVisuals = false;
        textBox.FocusVisualPrimaryThickness = new Microsoft.UI.Xaml.Thickness(0);
        textBox.FocusVisualSecondaryThickness = new Microsoft.UI.Xaml.Thickness(0);
        textBox.FocusVisualPrimaryBrush = transparent;
        textBox.FocusVisualSecondaryBrush = transparent;
    }

    private static void TryApplyBorderlessComboBox(Microsoft.UI.Xaml.Controls.ComboBox comboBox)
    {
        try
        {
            ApplyBorderlessComboBox(comboBox);
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"ComboBox styling error: {ex.Message}");
        }
    }

    private static void ApplyBorderlessComboBox(Microsoft.UI.Xaml.Controls.ComboBox comboBox)
    {
        var transparent = new Microsoft.UI.Xaml.Media.SolidColorBrush(Microsoft.UI.Colors.Transparent);
        comboBox.BorderThickness = new Microsoft.UI.Xaml.Thickness(0);
        comboBox.Padding = new Microsoft.UI.Xaml.Thickness(0);
        comboBox.Background = transparent;
        comboBox.BorderBrush = transparent;
    }
#endif
}
