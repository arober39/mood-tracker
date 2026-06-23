using LaunchDarkly.Observability;
using LaunchDarkly.Sdk;
using LaunchDarkly.Sdk.Client;
using LaunchDarkly.Sdk.Client.Integrations;
using LaunchDarkly.SessionReplay;
using Microsoft.Extensions.Logging;
using mood_tracker.Observability;
using mood_tracker.Pages;
using mood_tracker.Services;
using mood_tracker.ViewModels;

namespace mood_tracker;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        builder
            .UseMauiApp<App>()
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf",  "OpenSansRegular");
                fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
            });

        // ---------- LaunchDarkly client + observability + session replay ----------
        var ldConfig = Configuration.Builder(
                LDConfig.MobileKey,
                ConfigurationBuilder.AutoEnvAttributes.Enabled)
            .Plugins(new PluginConfigurationBuilder()
                .Add(new ObservabilityPlugin(new ObservabilityOptions(
                    isEnabled: true,
                    serviceName: LDConfig.ServiceName)))
                .Add(new SessionReplayPlugin(new SessionReplayOptions(
                    isEnabled: true,
                    privacy: new SessionReplayOptions.PrivacyOptions(
                        maskTextInputs: true,
                        maskWebViews: false,
                        maskLabels: false,
                        maskImages: false))))
            ).Build();

        var context  = Context.New("demo-user");
        var ldClient = LdClient.Init(ldConfig, context, TimeSpan.FromSeconds(10));
        builder.Services.AddSingleton(ldClient);

        // ---------- Domain services ----------
        builder.Services.AddSingleton<IEntryStore, EntryStore>();

        // ---------- ViewModels ----------
        builder.Services.AddTransient<LoginViewModel>();
        builder.Services.AddTransient<OnboardingViewModel>();
        builder.Services.AddTransient<TodayViewModel>();
        builder.Services.AddTransient<CheckInViewModel>();
        builder.Services.AddTransient<HistoryViewModel>();
        builder.Services.AddTransient<EntryDetailViewModel>();

        // ---------- Pages ----------
        builder.Services.AddTransient<LoginPage>();
        builder.Services.AddTransient<OnboardingPage>();
        builder.Services.AddTransient<TodayPage>();
        builder.Services.AddTransient<CheckInPage>();
        builder.Services.AddTransient<HistoryPage>();
        builder.Services.AddTransient<EntryDetailPage>();

#if DEBUG
        builder.Logging.AddDebug();
#endif

        return builder.Build();
    }
}
