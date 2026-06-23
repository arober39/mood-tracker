namespace mood_tracker.Models;

public enum MoodScore
{
    Rough = 1,
    Low   = 2,
    Okay  = 3,
    Good  = 4,
    Great = 5,
}

public static class MoodScoreExtensions
{
    public static string Label(this MoodScore score) => score switch
    {
        MoodScore.Rough => "Rough",
        MoodScore.Low   => "Low",
        MoodScore.Okay  => "Okay",
        MoodScore.Good  => "Good",
        MoodScore.Great => "Great",
        _ => "",
    };

    // Soft, low-chroma pastels — translated from oklch in the Tend mockup.
    public static Color Color(this MoodScore score) => score switch
    {
        MoodScore.Rough => Microsoft.Maui.Graphics.Color.FromArgb("#A8B8C9"), // dusty blue
        MoodScore.Low   => Microsoft.Maui.Graphics.Color.FromArgb("#B6AECC"), // lavender
        MoodScore.Okay  => Microsoft.Maui.Graphics.Color.FromArgb("#B7CDB3"), // sage
        MoodScore.Good  => Microsoft.Maui.Graphics.Color.FromArgb("#D4C896"), // sand
        MoodScore.Great => Microsoft.Maui.Graphics.Color.FromArgb("#E8B898"), // apricot
        _ => Microsoft.Maui.Graphics.Color.FromArgb("#E7E2D8"),
    };
}
