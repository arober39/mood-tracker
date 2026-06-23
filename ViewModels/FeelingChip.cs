using CommunityToolkit.Mvvm.ComponentModel;

namespace mood_tracker.ViewModels;

public partial class FeelingChip : ObservableObject
{
    public string Name { get; init; } = "";

    [ObservableProperty] private bool isSelected;

    public Color Background => IsSelected
        ? Color.FromArgb("#2A2823")
        : Color.FromArgb("#F2EFE8");

    public Color TextColor => IsSelected
        ? Color.FromArgb("#FAF8F4")
        : Color.FromArgb("#2A2823");

    partial void OnIsSelectedChanged(bool value)
    {
        OnPropertyChanged(nameof(Background));
        OnPropertyChanged(nameof(TextColor));
    }
}
