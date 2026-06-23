using mood_tracker.Pages;

namespace mood_tracker;

public partial class AppShell : Shell
{
    public AppShell()
    {
        InitializeComponent();

        // Modal/secondary routes pushed onto the stack from Today / History.
        Routing.RegisterRoute("checkin", typeof(CheckInPage));
        Routing.RegisterRoute("entry",   typeof(EntryDetailPage));
    }
}
