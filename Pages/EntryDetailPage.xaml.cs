using mood_tracker.ViewModels;

namespace mood_tracker.Pages;

public partial class EntryDetailPage : ContentPage
{
    public EntryDetailPage(EntryDetailViewModel vm)
    {
        InitializeComponent();
        BindingContext = vm;
    }
}
