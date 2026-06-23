using mood_tracker.ViewModels;

namespace mood_tracker.Pages;

public partial class CheckInPage : ContentPage
{
    readonly CheckInViewModel _vm;

    public CheckInPage(CheckInViewModel vm)
    {
        InitializeComponent();
        BindingContext = _vm = vm;
        _vm.PropertyChanged += OnVmPropertyChanged;
        UpdateStepVisibility();
    }

    void OnVmPropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(CheckInViewModel.Step))
            UpdateStepVisibility();
    }

    void UpdateStepVisibility()
    {
        StepMood.IsVisible     = _vm.Step == 0;
        StepFeelings.IsVisible = _vm.Step == 1;
        StepNote.IsVisible     = _vm.Step == 2;
    }
}
