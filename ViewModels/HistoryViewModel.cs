using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using LaunchDarkly.Observability;
using mood_tracker.Models;
using mood_tracker.Services;

namespace mood_tracker.ViewModels;

public partial class HistoryViewModel : BaseViewModel
{
    readonly IEntryStore _store;

    [ObservableProperty] private string monthLabel = "";
    [ObservableProperty] private int year;
    [ObservableProperty] private int month;

    public ObservableCollection<CalendarCell> Cells { get; } = new();
    public ObservableCollection<MoodLegendItem> Legend { get; } = new();

    public HistoryViewModel(IEntryStore store)
    {
        _store = store;
        Title  = "History";

        foreach (MoodScore s in Enum.GetValues<MoodScore>())
            Legend.Add(new MoodLegendItem { Score = s });

        var today = DateOnly.FromDateTime(DateTime.Today);
        Year  = today.Year;
        Month = today.Month;
        BuildCalendar();
    }

    public void Refresh() => BuildCalendar();

    [RelayCommand]
    private void PrevMonth()
    {
        var d = new DateTime(Year, Month, 1).AddMonths(-1);
        Year  = d.Year;
        Month = d.Month;
        BuildCalendar();
    }

    [RelayCommand]
    private void NextMonth()
    {
        var d = new DateTime(Year, Month, 1).AddMonths(1);
        Year  = d.Year;
        Month = d.Month;
        BuildCalendar();
    }

    [RelayCommand]
    private async Task OpenCellAsync(CalendarCell cell)
    {
        if (cell?.Date is null || !cell.IsLogged) return;
        await Shell.Current.GoToAsync($"entry?date={cell.Date:yyyy-MM-dd}");
    }

    void BuildCalendar()
    {
        using var span = LDObserve.StartActiveSpan("HistoryPage.BuildCalendar");

        Cells.Clear();
        var first = new DateOnly(Year, Month, 1);
        MonthLabel = first.ToString("MMMM yyyy");

        // Week starts Monday
        int leading = ((int)first.DayOfWeek + 6) % 7;
        for (int i = 0; i < leading; i++)
            Cells.Add(new CalendarCell());

        var today = DateOnly.FromDateTime(DateTime.Today);
        int daysInMonth = DateTime.DaysInMonth(Year, Month);
        for (int d = 1; d <= daysInMonth; d++)
        {
            var date  = new DateOnly(Year, Month, d);
            var entry = _store.GetByDate(date);
            Cells.Add(new CalendarCell
            {
                Date     = date,
                IsToday  = date == today,
                IsFuture = date > today,
                IsLogged = entry is not null,
                Score    = entry?.Score,
            });
        }

        span.SetAttribute("month",  MonthLabel);
        span.SetAttribute("logged", Cells.Count(c => c.IsLogged));
    }
}
