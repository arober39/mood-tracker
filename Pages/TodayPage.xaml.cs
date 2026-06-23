using mood_tracker.ViewModels;

namespace mood_tracker.Pages;

public partial class TodayPage : ContentPage
{
    readonly TodayViewModel _vm;

    public TodayPage(TodayViewModel vm)
    {
        InitializeComponent();
        BindingContext = _vm = vm;
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        // Refresh after returning from check-in so a brand new entry shows up.
        _vm.Refresh();
    }
}
