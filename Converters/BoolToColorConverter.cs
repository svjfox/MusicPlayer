using System.Globalization;

public class BoolToColorConverter : IValueConverter
{
    /// <summary>
    /// Конвертирует булево значение в цвет.
    /// </summary>
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is bool boolValue && boolValue && parameter is Color color)
        {
            return color;
        }
        return Colors.Gray;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}