using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using LaunchDarkly.Observability;

namespace mood_tracker.ViewModels;

public partial class LoginViewModel : BaseViewModel
{
    const string HasOnboardedKey = "tend.hasOnboarded";

    [ObservableProperty]
    private string username = "";

    [ObservableProperty]
    private string password = "";

    public LoginViewModel() { Title = "Sign in"; }

    [RelayCommand]
    private async Task SignInAsync()
    {
        using var span = LDObserve.StartActiveSpan("Login");
        span.SetAttribute("username.length", Username.Length);

        var route = Preferences.Default.Get(HasOnboardedKey, false)
            ? "//main/today"
            : "//onboarding";

        await Shell.Current.GoToAsync(route);
    }
}
