using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using LaunchDarkly.Observability;
using mood_tracker.Models;
using mood_tracker.Services;

namespace mood_tracker.ViewModels;

public partial class CheckInViewModel : BaseViewModel
{
    readonly IEntryStore _store;

    [ObservableProperty] private int       step;          // 0 mood · 1 feelings · 2 note
    [ObservableProperty] private MoodScore selectedScore = MoodScore.Okay;
    [ObservableProperty] private string    note = "";

    [ObservableProperty] private string promptHeadline = "";
    [ObservableProperty] private string promptSubtitle = "";
    [ObservableProperty] private string continueLabel  = "Continue";
    [ObservableProperty] private Color   tintColor = Colors.Transparent;

    public ObservableCollection<MoodOption> MoodOptions { get; } = new();
    public ObservableCollection<FeelingChip> Feelings    { get; } = new();

    public CheckInViewModel(IEntryStore store)
    {
        _store = store;
        Title  = "Check in";

        foreach (MoodScore s in Enum.GetValues<MoodScore>())
            MoodOptions.Add(new MoodOption { Score = s });

        foreach (var t in FeelingTag.All)
            Feelings.Add(new FeelingChip { Name = t });

        UpdateForStep();
    }

    partial void OnSelectedScoreChanged(MoodScore value) => UpdateForStep();
    partial void OnStepChanged(int value) => UpdateForStep();

    void UpdateForStep()
    {
        TintColor = WashFor(SelectedScore);
        (PromptHeadline, PromptSubtitle, ContinueLabel) = Step switch
        {
            0 => ("How are you, really?", "No wrong answer. Just notice.", "Continue"),
            1 => ("What's it feel like?",  "Pick anything that fits.",      "Continue"),
            2 => ("Anything you want to say?", "A line is enough. Skip if you'd rather.", "Save"),
            _ => ("", "", "Save"),
        };
    }

    [RelayCommand]
    private void SelectScore(MoodOption opt)
    {
        if (opt is null) return;
        SelectedScore = opt.Score;
    }

    [RelayCommand]
    private void ToggleFeeling(FeelingChip chip)
    {
        if (chip is null) return;
        chip.IsSelected = !chip.IsSelected;
    }

    [RelayCommand]
    private async Task ContinueAsync()
    {
        if (Step < 2)
        {
            Step++;
            return;
        }

        using var span = LDObserve.StartActiveSpan("CheckIn.Save");
        span.SetAttribute("score",        (int)SelectedScore);
        span.SetAttribute("feeling.count", Feelings.Count(f => f.IsSelected));
        span.SetAttribute("note.length",   Note?.Length ?? 0);

        var now = DateTime.Now;
        var entry = new MoodEntry
        {
            Date     = DateOnly.FromDateTime(now).ToString("yyyy-MM-dd"),
            Score    = SelectedScore,
            Feelings = Feelings.Where(f => f.IsSelected).Select(f => f.Name).ToList(),
            Note     = Note ?? "",
            Time     = now.ToString("h:mm tt"),
        };
        _store.Upsert(entry);

        await Shell.Current.GoToAsync("..");
    }

    [RelayCommand]
    private async Task BackAsync()
    {
        if (Step > 0) Step--;
        else await Shell.Current.GoToAsync("..");
    }

    [RelayCommand]
    private static async Task CloseAsync() => await Shell.Current.GoToAsync("..");

    // A very faint tint of the chosen mood color for the page background.
    static Color WashFor(MoodScore s)
    {
        var c = s.Color();
        return Color.FromRgba(c.Red, c.Green, c.Blue, 0.18f);
    }
}
