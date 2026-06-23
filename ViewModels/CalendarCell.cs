using mood_tracker.Models;

namespace mood_tracker.ViewModels;

public class CalendarCell
{
    public DateOnly? Date { get; init; }            // null for leading blanks
    public bool      IsToday { get; init; }
    public bool      IsFuture { get; init; }
    public bool      IsLogged { get; init; }
    public MoodScore? Score { get; init; }

    public string Day => Date?.Day.ToString() ?? "";
    public bool   HasDay => Date is not null;

    public Color FillColor =>
        IsLogged && Score is { } s
            ? s.Color()
            : Colors.Transparent;

    public Color TextColor =>
        IsLogged ? Colors.White
        : IsFuture ? Color.FromArgb("#CFCABE")
        :            Color.FromArgb("#2A2823");
}
