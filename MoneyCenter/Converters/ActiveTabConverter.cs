using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoneyCenter.Converters
{
    public class ActiveTabConverter : IValueConverter
    {
        public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            if (value is string activeTab && parameter is string tabName && activeTab == tabName)
            {
                return Color.FromArgb("#E7E8D0");
            }

            return Color.FromArgb("#F0E8D6");
        }

        public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

}
