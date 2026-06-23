using LaunchDarkly.SessionReplay;
using mood_tracker.ViewModels;

namespace mood_tracker.Pages;

public partial class LoginPage : ContentPage
{
    public LoginPage(LoginViewModel vm)
    {
        InitializeComponent();
        BindingContext = vm;
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        // Text inputs are masked by default; calling LDMask() here is
        // belt-and-suspenders and makes the masking unmistakable when
        // watching a session replay.
        PasswordEntry.LDMask();
    }
}
