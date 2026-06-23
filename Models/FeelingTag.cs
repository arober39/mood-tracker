namespace mood_tracker.Models;

public static class FeelingTag
{
    public static readonly IReadOnlyList<string> All = new[]
    {
        "Calm", "Grateful", "Tired", "Anxious", "Hopeful", "Overwhelmed",
        "Content", "Energized", "Proud", "Stressed", "Peaceful", "Focused",
        "Lonely", "Restless",
    };
}
