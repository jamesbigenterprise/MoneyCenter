using System.Windows.Input;
using Microsoft.Maui.Controls.Shapes;
using MoneyCenter.Views.Icons;

namespace MoneyCenter.Views.Controls;

public class NavTabsView : ContentView
{
    private readonly Grid _grid = new();
    private readonly List<NavTabItem> _tabs =
    [
        new("dashboard", "Dashboard", MaterialIcon.Home),
        new("expenses", "Expenses", MaterialIcon.List),
        new("savings", "Savings", MaterialIcon.Savings),
        new("budgets", "Budgets", MaterialIcon.Account_balance),
        new("settings", "Settings", MaterialIcon.Settings)
    ];

    public static readonly BindableProperty ActiveViewProperty = BindableProperty.Create(
        nameof(ActiveView),
        typeof(string),
        typeof(NavTabsView),
        "dashboard",
        propertyChanged: (bindable, _, _) => ((NavTabsView)bindable).BuildTabs());

    public static readonly BindableProperty NavigateCommandProperty = BindableProperty.Create(
        nameof(NavigateCommand),
        typeof(ICommand),
        typeof(NavTabsView),
        propertyChanged: (bindable, _, _) => ((NavTabsView)bindable).BuildTabs());

    public static readonly BindableProperty IsDesktopProperty = BindableProperty.Create(
        nameof(IsDesktop),
        typeof(bool),
        typeof(NavTabsView),
        true,
        propertyChanged: (bindable, _, _) => ((NavTabsView)bindable).BuildTabs());

    public string ActiveView
    {
        get => (string)GetValue(ActiveViewProperty);
        set => SetValue(ActiveViewProperty, value);
    }

    public ICommand? NavigateCommand
    {
        get => (ICommand?)GetValue(NavigateCommandProperty);
        set => SetValue(NavigateCommandProperty, value);
    }

    public bool IsDesktop
    {
        get => (bool)GetValue(IsDesktopProperty);
        set => SetValue(IsDesktopProperty, value);
    }

    public NavTabsView()
    {
        Content = _grid;
        BuildTabs();
    }

    private void BuildTabs()
    {
        _grid.Children.Clear();
        _grid.ColumnDefinitions.Clear();

        foreach (var _ in _tabs)
        {
            _grid.ColumnDefinitions.Add(new ColumnDefinition(GridLength.Star));
        }

        _grid.HeightRequest = IsDesktop ? 40 : 58;

        for (var i = 0; i < _tabs.Count; i++)
        {
            var tab = CreateTab(_tabs[i]);
            Grid.SetColumn(tab, i);
            _grid.Children.Add(tab);
        }
    }

    private Border CreateTab(NavTabItem item)
    {
        var isActive = string.Equals(ActiveView, item.Key, StringComparison.OrdinalIgnoreCase);
        var primary = GetResourceColor("Primary");
        var surface = GetResourceColor("AppSurface1Light");
        var navActive = GetResourceColor("AppNavActiveLight");
        var muted = GetResourceColor("AppMutedForegroundLight");

        var tab = new Border
        {
            BackgroundColor = isActive ? navActive : surface,
            Stroke = IsDesktop && isActive ? primary : Colors.Transparent,
            StrokeThickness = IsDesktop ? 1 : 0,
            StrokeShape = IsDesktop ? new RoundRectangle { CornerRadius = 6 } : new Rectangle()
        };

        var tap = new TapGestureRecognizer
        {
            Command = NavigateCommand,
            CommandParameter = item.Key
        };
        tab.GestureRecognizers.Add(tap);

        tab.Content = IsDesktop
            ? CreateDesktopLabel(item.Title, isActive ? primary : muted)
            : CreateMobileContent(item, isActive ? primary : muted);

        return tab;
    }

    private static Label CreateDesktopLabel(string title, Color color)
    {
        return new Label
        {
            Text = title,
            TextColor = color,
            HorizontalOptions = LayoutOptions.Center,
            VerticalOptions = LayoutOptions.Center
        };
    }

    private static VerticalStackLayout CreateMobileContent(NavTabItem item, Color color)
    {
        return new VerticalStackLayout
        {
            Spacing = 1,
            HorizontalOptions = LayoutOptions.Center,
            VerticalOptions = LayoutOptions.Center,
            Children =
            {
                new Label
                {
                    Text = item.Icon,
                    FontFamily = nameof(MaterialIcon),
                    FontSize = 20,
                    HorizontalTextAlignment = TextAlignment.Center,
                    TextColor = color
                },
                new Label
                {
                    Text = item.Title,
                    FontSize = 12,
                    TextColor = color
                }
            }
        };
    }

    private Color GetResourceColor(string key)
    {
        if (Application.Current?.Resources.TryGetValue(key, out var value) == true && value is Color color)
        {
            return color;
        }

        return Colors.Transparent;
    }

    private sealed record NavTabItem(string Key, string Title, string Icon);
}
