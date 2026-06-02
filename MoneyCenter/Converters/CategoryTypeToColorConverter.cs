using System;
using System.Globalization;
using Microsoft.Maui.Graphics;
using Microsoft.Maui.Controls;

namespace MoneyCenter.Converters
{
    public class CategoryTypeToColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var type = value as string;
            if (string.Equals(type, "expense", StringComparison.OrdinalIgnoreCase))
                return Color.FromArgb("#DC3545"); // Destructive red matching Tailwind
            if (string.Equals(type, "savings", StringComparison.OrdinalIgnoreCase))
                return Color.FromArgb("#D3C0A3"); // Tan/Yellowish matching Savings pill
            
            return Color.FromArgb("#6C757D"); // Default gray for flex/recurring
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
