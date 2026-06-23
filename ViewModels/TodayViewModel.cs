using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using LaunchDarkly.Observability;
using mood_tracker.Models;
using mood_tracker.Services;

namespace mood_tracker.ViewModels;

public partial class TodayViewModel : BaseViewModel
{
    readonly IEntryStore _store;

    [ObservableProperty] private string greeting = "Hello";
    [ObservableProperty] private string dateLine = "";
    [ObservableProperty] private bool   hasTodayEntry;
    [ObservableProperty] private MoodEntry? todayEntry;
    [ObservableProperty] private string weeklyInsight = "";
    [ObservableProperty] private string todayLabel = "";

    public ObservableCollection<DayDot> Week { get; } = new();
    public ObservableCollection<ReflectionRow> Reflections { get; } = new();

    public TodayViewModel(IEntryStore store)
    {
        _store = store;
        Title  = "Today";
        Refresh();
    }

    public void Refresh()
    {
        using var span = LDObserve.StartActiveSpan("TodayPage.Refresh");

        var now   = DateTime.Now;
        var today = DateOnly.FromDateTime(now);
        Greeting  = $"{TimeOfDayGreeting(now)}";
        DateLine  = today.ToString("dddd · MMMM d").ToUpperInvariant();

        TodayEntry    = _store.GetByDate(today);
        HasTodayEntry = TodayEntry is not null;
        TodayLabel    = TodayEntry?.Score.Label() ?? "";

        BuildWeek(today);
        BuildReflections();
        BuildInsight(today);

        span.SetAttribute("today.logged",    HasTodayEntry);
        span.SetAttribute("reflection.count", Reflections.Count);
    }

    void BuildWeek(DateOnly today)
    {
        // Week starts Monday — matches the design.
        int offsetToMon = ((int)today.DayOfWeek + 6) % 7;
        var monday = today.AddDays(-offsetToMon);

        Week.Clear();
        for (int i = 0; i < 7; i++)
        {
            var d = monday.AddDays(i);
            var entry = _store.GetByDate(d);
            Week.Add(new DayDot
            {
                Date     = d,
                IsToday  = d == today,
                IsFuture = d > today,
                IsLogged = entry is not null,
                Score    = entry?.Score,
            });
        }
    }

    void BuildReflections()
    {
        Reflections.Clear();
        foreach (var e in _store.GetAll().Take(8))
            Reflections.Add(new ReflectionRow { Entry = e });
    }

    void BuildInsight(DateOnly today)
    {
        int offsetToMon = ((int)today.DayOfWeek + 6) % 7;
        var monday = today.AddDays(-offsetToMon);

        var scores = Enumerable.Range(0, 7)
            .Select(i => _store.GetByDate(monday.AddDays(i)))
            .Where(e => e is not null)
            .Select(e => (int)e!.Score)
            .ToList();

        if (scores.Count == 0)
        {
            WeeklyInsight = "A fresh week";
            return;
        }

        var avg = scores.Average();
        WeeklyInsight = avg switch
        {
            >= 4    => "A brighter stretch",
            >= 3    => "Mostly steady",
            _       => "A heavier few days",
        };
    }

    [RelayCommand]
    private static async Task CheckInAsync() =>
        await Shell.Current.GoToAsync("checkin");

    [RelayCommand]
    private static async Task OpenEntryAsync(ReflectionRow row)
    {
        if (row?.Entry is null) return;
        await Shell.Current.GoToAsync($"entry?date={row.Entry.Date}");
    }

    static string TimeOfDayGreeting(DateTime t) => t.Hour switch
    {
        <  5 => "Hello, night owl",
        < 12 => "Good morning",
        < 17 => "Good afternoon",
        < 22 => "Good evening",
        _    => "Settling in",
    };
}
