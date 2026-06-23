using mood_tracker.Models;

namespace mood_tracker.ViewModels;

// A single row in the Today "Recent reflections" list.
public class ReflectionRow
{
    public MoodEntry Entry { get; init; } = new();
    public string Label => Entry.Score.Label();
    public Color  Color => Entry.Score.Color();
    public string DateDisplay => Entry.DateValue.ToString("ddd, MMM d");
    public string Note => string.IsNullOrWhiteSpace(Entry.Note) ? "(no note)" : Entry.Note;
}
