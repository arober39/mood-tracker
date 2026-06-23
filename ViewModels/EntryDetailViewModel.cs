using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using LaunchDarkly.Observability;
using mood_tracker.Models;
using mood_tracker.Services;

namespace mood_tracker.ViewModels;

[QueryProperty(nameof(Date), "date")]
public partial class EntryDetailViewModel : BaseViewModel
{
    readonly IEntryStore _store;

    [ObservableProperty] private string date = "";
    [ObservableProperty] private MoodEntry? entry;
    [ObservableProperty] private string weekday = "";
    [ObservableProperty] private string dateLine = "";
    [ObservableProperty] private string label = "";
    [ObservableProperty] private string time = "";
    [ObservableProperty] private string note = "";
    [ObservableProperty] private bool   hasNote;
    [ObservableProperty] private MoodScore score = MoodScore.Okay;

    public List<string> Feelings { get; private set; } = new();

    public EntryDetailViewModel(IEntryStore store)
    {
        _store = store;
        Title  = "Entry";
    }

    partial void OnDateChanged(string value) => Load(value);

    void Load(string iso)
    {
        if (!DateOnly.TryParse(iso, out var d)) return;
        using var span = LDObserve.StartActiveSpan("EntryDetailPage.Load");
        span.SetAttribute("date", iso);

        Entry    = _store.GetByDate(d);
        Weekday  = d.ToString("dddd").ToUpperInvariant();
        DateLine = d.ToString("MMMM d");

        if (Entry is null)
        {
            Score   = MoodScore.Okay;
            Label   = "No entry";
            Time    = "";
            HasNote = false;
            Note    = "No entry for this day yet.";
            Feelings = new();
            OnPropertyChanged(nameof(Feelings));
            return;
        }

        Score    = Entry.Score;
        Label    = Entry.Score.Label();
        Time     = Entry.Time;
        HasNote  = !string.IsNullOrWhiteSpace(Entry.Note);
        Note     = HasNote ? Entry.Note : "No note for this day — just a quiet check-in.";
        Feelings = Entry.Feelings.ToList();
        OnPropertyChanged(nameof(Feelings));
    }

    [RelayCommand]
    private static async Task BackAsync() => await Shell.Current.GoToAsync("..");
}
