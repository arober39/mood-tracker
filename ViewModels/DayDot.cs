using mood_tracker.Models;

namespace mood_tracker.ViewModels;

// A single cell in the Today week-strip — a labeled circle that either shows
// the logged mood color or a faint placeholder.
public class DayDot
{
    public DateOnly Date { get; init; }
    public string DayNumber => Date.Day.ToString();
    public string Weekday   => Date.ToString("ddd")[..1].ToUpperInvariant();
    public bool   IsToday   { get; init; }
    public bool   IsFuture  { get; init; }
    public bool   IsLogged  { get; init; }
    public MoodScore? Score { get; init; }

    public Color FillColor =>
        IsLogged && Score is { } s ? s.Color()
        : IsFuture                  ? Color.FromArgb("#F2EFE8")
        :                             Color.FromArgb("#E6E2D8");

    public Color TextColor =>
        IsLogged ? Colors.White
        : IsFuture ? Color.FromArgb("#A8A296")
        :            Color.FromArgb("#6F6A60");
}
