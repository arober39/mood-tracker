using mood_tracker.Models;

namespace mood_tracker.ViewModels;

public class MoodLegendItem
{
    public MoodScore Score { get; init; }
    public string Label => Score.Label();
    public Color  Color => Score.Color();
}
