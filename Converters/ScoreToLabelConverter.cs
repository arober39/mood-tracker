using System.Globalization;
using mood_tracker.Models;

namespace mood_tracker.Converters;

public class ScoreToLabelConverter : IValueConverter
{
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture) =>
        value is MoodScore s ? s.Label() : "";

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture) =>
        throw new NotSupportedException();
}
