using System.Globalization;

namespace MoneyCenter.Converters;

public class BoolToObjectConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (parameter is not string options)
            return null;

        var parts = options.Split('|');
        if (parts.Length != 2)
            return null;

        var selected = value is true ? parts[0] : parts[1];

        if (targetType == typeof(Thickness))
            return ParseThickness(selected);

        return selected;
    }

    private static Thickness ParseThickness(string value)
    {
        var parts = value.Split(',', StringSplitOptions.TrimEntries);
        var numbers = parts.Select(p => double.TryParse(p, CultureInfo.InvariantCulture, out var n) ? n : 0).ToArray();

        switch (numbers.Length)
        {
            case 1:
                return new Thickness(numbers[0]);
            case 2:
                return new Thickness(numbers[0], numbers[1]);
            case 4:
                return new Thickness(numbers[0], numbers[1], numbers[2], numbers[3]);
            default:
                return new Thickness(0);
        }
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
