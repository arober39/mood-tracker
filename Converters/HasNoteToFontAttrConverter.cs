using System.Globalization;

namespace mood_tracker.Converters;

public class HasNoteToFontAttrConverter : IValueConverter
{
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture) =>
        value is bool b && b ? FontAttributes.None : FontAttributes.Italic;

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture) =>
        throw new NotSupportedException();
}
