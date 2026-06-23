namespace mood_tracker.Models;

public class MoodEntry
{
    // ISO date "YYYY-MM-DD" — one entry per day, keyed by this.
    public string Date { get; set; } = "";
    public MoodScore Score { get; set; } = MoodScore.Okay;
    public List<string> Feelings { get; set; } = new();
    public string Note { get; set; } = "";
    public string Time { get; set; } = "";

    public DateOnly DateValue =>
        DateOnly.TryParse(Date, out var d) ? d : DateOnly.FromDateTime(DateTime.Today);
}
