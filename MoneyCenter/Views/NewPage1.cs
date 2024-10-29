using Microsoft.Maui.Controls.Shapes;
using CommunityToolkit.Maui.Markup;

using MoneyCenter.Views.Controls;
using FmgLib.MauiMarkup;
using Microsoft.Maui;

namespace MoneyCenter.Views;

public class NewPage1 : ContentPage
{
    #region Collpsable
    private readonly AccordionControl _Top =
    new AccordionControl
    {
        Stroke = Colors.Red,
        StrokeShape = new RoundRectangle { CornerRadius = new CornerRadius(5, 5,0,0 )},
        Header = new Label { Text = "Header" },
        AccordionContent = new Label { Text = "Content",Padding=16 }
    };
    #endregion Collpsable
    public NewPage1()
	{
		Content = new VerticalStackLayout
		{
			Children = {
                _Top
            }
		};
	}
}