using CommunityToolkit.Maui.Views;
using Microsoft.Maui.Controls.Shapes;

namespace MoneyCenter.Views.Controls;

public class ConfirmationPopup : Popup<bool>
{
    public ConfirmationPopup(string title, string message, string confirmText = "Delete")
    {
        CanBeDismissedByTappingOutsideOfPopup = true;

        Content = new Border
        {
            WidthRequest = 420,
            MaximumWidthRequest = 420,
            Padding = new Thickness(24),
            BackgroundColor = Color.FromArgb("#F2EADF"),
            Stroke = Color.FromArgb("#CFC2A9"),
            StrokeThickness = 1,
            StrokeShape = new RoundRectangle { CornerRadius = 8 },
            Content = new VerticalStackLayout
            {
                Spacing = 18,
                Children =
                {
                    new Label
                    {
                        Text = title,
                        FontSize = 20,
                        FontAttributes = FontAttributes.Bold,
                        TextColor = Color.FromArgb("#5A4D39")
                    },
                    new Label
                    {
                        Text = message,
                        FontSize = 14,
                        TextColor = Color.FromArgb("#75633F")
                    },
                    new Grid
                    {
                        ColumnDefinitions =
                        {
                            new ColumnDefinition(GridLength.Star),
                            new ColumnDefinition(GridLength.Auto),
                            new ColumnDefinition(GridLength.Auto)
                        },
                        ColumnSpacing = 10,
                        Children =
                        {
                            CreateCancelButton(),
                            CreateConfirmButton(confirmText)
                        }
                    }
                }
            }
        };
    }

    private Button CreateCancelButton()
    {
        var button = new Button
        {
            Text = "Cancel",
            BackgroundColor = Color.FromArgb("#F4EEDC"),
            TextColor = Color.FromArgb("#5A4D39"),
            BorderColor = Color.FromArgb("#CFC2A9"),
            BorderWidth = 1,
            CornerRadius = 6,
            Padding = new Thickness(16, 8),
            MinimumHeightRequest = 40
        };
        Grid.SetColumn(button, 1);
        button.Clicked += async (_, _) => await CloseAsync(false);
        return button;
    }

    private Button CreateConfirmButton(string confirmText)
    {
        var button = new Button
        {
            Text = confirmText,
            BackgroundColor = Color.FromArgb("#EF4444"),
            TextColor = Colors.White,
            CornerRadius = 6,
            Padding = new Thickness(16, 8),
            MinimumHeightRequest = 40
        };
        Grid.SetColumn(button, 2);
        button.Clicked += async (_, _) => await CloseAsync(true);
        return button;
    }
}
