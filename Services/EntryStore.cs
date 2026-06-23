using System.Text.Json;
using mood_tracker.Models;

namespace mood_tracker.Services;

public class EntryStore : IEntryStore
{
    const string Key = "tend.entries.v1";
    const string SeededKey = "tend.seeded.v1";

    static readonly JsonSerializerOptions JsonOpts = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        WriteIndented = false,
    };

    readonly Dictionary<string, MoodEntry> _byDate = new();

    public EntryStore()
    {
        Load();
        if (!Preferences.Default.Get(SeededKey, false))
        {
            SeedDemoEntries();
            Preferences.Default.Set(SeededKey, true);
            Save();
        }
    }

    public IReadOnlyList<MoodEntry> GetAll() =>
        _byDate.Values
            .OrderByDescending(e => e.Date, StringComparer.Ordinal)
            .ToList();

    public MoodEntry? GetByDate(DateOnly date) =>
        _byDate.TryGetValue(date.ToString("yyyy-MM-dd"), out var e) ? e : null;

    public void Upsert(MoodEntry entry)
    {
        _byDate[entry.Date] = entry;
        Save();
    }

    void Load()
    {
        var json = Preferences.Default.Get(Key, string.Empty);
        if (string.IsNullOrWhiteSpace(json)) return;
        try
        {
            var list = JsonSerializer.Deserialize<List<MoodEntry>>(json, JsonOpts) ?? new();
            foreach (var e in list) _byDate[e.Date] = e;
        }
        catch
        {
            // Corrupt cache — start clean rather than crash on launch.
            _byDate.Clear();
        }
    }

    void Save()
    {
        var json = JsonSerializer.Serialize(_byDate.Values, JsonOpts);
        Preferences.Default.Set(Key, json);
    }

    void SeedDemoEntries()
    {
        // ~3 weeks of plausible entries ending yesterday so today still feels like
        // an open check-in slot.
        var today = DateOnly.FromDateTime(DateTime.Today);
        var seed = new (int daysBack, MoodScore score, string[] feelings, string note, string time)[]
        {
            (1,  MoodScore.Good,  new[]{"Proud","Focused"},   "Proud of how I handled that feedback without spiraling.", "9:12 AM"),
            (2,  MoodScore.Okay,  new[]{"Tired"},             "",                                                       "8:50 AM"),
            (3,  MoodScore.Great, new[]{"Peaceful","Grateful"},"Slow morning, tea before email. I want more of these.",  "8:05 AM"),
            (4,  MoodScore.Low,   new[]{"Overwhelmed"},       "Too many tabs, too many threads. Need a real break.",    "1:40 PM"),
            (5,  MoodScore.Okay,  new[]{"Focused"},           "",                                                       "10:20 AM"),
            (6,  MoodScore.Good,  new[]{"Content","Calm"},    "Walked at lunch. Cleared my head.",                      "12:55 PM"),
            (7,  MoodScore.Great, new[]{"Energized","Hopeful"},"Plan for the week feels doable for once.",              "8:40 AM"),
            (8,  MoodScore.Rough, new[]{"Anxious","Restless"},"Couldn't sleep. Tomorrow I rest, not push.",             "11:15 PM"),
            (9,  MoodScore.Okay,  new[]{"Tired"},             "",                                                       "9:00 AM"),
            (10, MoodScore.Good,  new[]{"Grateful"},          "Kind text from a friend mattered more than I expected.", "7:55 PM"),
            (11, MoodScore.Okay,  new[]{"Focused"},           "",                                                       "9:30 AM"),
            (12, MoodScore.Great, new[]{"Proud","Energized"}, "Shipped the thing. Letting myself feel it.",             "5:10 PM"),
            (13, MoodScore.Low,   new[]{"Lonely"},            "Long quiet day. Forgot to reach out to anyone.",         "8:20 PM"),
            (14, MoodScore.Good,  new[]{"Calm","Content"},    "",                                                       "8:35 AM"),
            (15, MoodScore.Okay,  new[]{"Stressed"},          "",                                                       "10:05 AM"),
            (16, MoodScore.Good,  new[]{"Hopeful"},           "Therapist reframed something I needed reframed.",        "6:45 PM"),
            (17, MoodScore.Great, new[]{"Peaceful"},          "",                                                       "9:00 AM"),
            (18, MoodScore.Okay,  new[]{"Tired","Focused"},   "Big week ahead. Going gentle on myself.",                "9:25 AM"),
            (19, MoodScore.Low,   new[]{"Overwhelmed"},       "",                                                       "2:10 PM"),
            (20, MoodScore.Good,  new[]{"Grateful","Calm"},   "Slow morning before the rush. Held onto it.",            "8:00 AM"),
        };

        foreach (var (daysBack, score, feelings, note, time) in seed)
        {
            var date = today.AddDays(-daysBack);
            var iso = date.ToString("yyyy-MM-dd");
            _byDate[iso] = new MoodEntry
            {
                Date = iso,
                Score = score,
                Feelings = feelings.ToList(),
                Note = note,
                Time = time,
            };
        }
    }
}
