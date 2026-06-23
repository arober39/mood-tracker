using CommunityToolkit.Mvvm.ComponentModel;

namespace mood_tracker.ViewModels;

public partial class BaseViewModel : ObservableObject
{
    [ObservableProperty]
    private bool isBusy;

    [ObservableProperty]
    private string title = "";
}
