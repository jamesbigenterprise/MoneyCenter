using Microsoft.Maui.Controls;
using MoneyCenter.Views.Icons;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace MoneyCenter.Views.Extensions
{
    public static class UIExtensions
    {
        public static VerticalStackLayout Spacing(this VerticalStackLayout layout, double spacing) 
        {
            layout.Spacing = spacing;
            return layout;
        }

        public static Image ApplyMaterialIcon(this Image image, string iconName, Color color, double size) 
        {
            image.Source = new FontImageSource
            {
                FontFamily = nameof(MaterialIcon),
                Glyph = iconName,
                Color = color,
                Size = size
            };
            return image;
        }

        public static BoxView MakeDivider(this BoxView boxView, Color color) 
        {
            boxView.Color = Colors.White;
            return boxView;
        }

        public static TapGestureRecognizer TapsRequired(this TapGestureRecognizer gesture, int taps)
        {
            gesture.NumberOfTapsRequired = taps;
            return gesture;
        }
    }
}
