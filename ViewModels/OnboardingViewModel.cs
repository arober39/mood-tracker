using CommunityToolkit.Mvvm.Input;
using LaunchDarkly.Observability;

namespace mood_tracker.ViewModels;

public partial class OnboardingViewModel : BaseViewModel
{
    const string HasOnboardedKey = "tend.hasOnboarded";

    public OnboardingViewModel() { Title = "Tend"; }

    [RelayCommand]
    private async Task BeginAsync()
    {
        using var span = LDObserve.StartActiveSpan("Onboarding.Begin");
        Preferences.Default.Set(HasOnboardedKey, true);
        await Shell.Current.GoToAsync("//main/today");
    }
}
