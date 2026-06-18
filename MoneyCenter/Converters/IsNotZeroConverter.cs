using System.Globalization;

namespace MoneyCenter.Converters;

public class IsNotZeroConverter : IValueConverter
{
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        switch (value)
        {
            case int i:
                return i != 0;
            case long l:
                return l != 0;
            case double d:
                return Math.Abs(d) > double.Epsilon;
            case decimal m:
                return m != 0;
            default:
                return false;
        }
    }

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotSupportedException();
    }
}
