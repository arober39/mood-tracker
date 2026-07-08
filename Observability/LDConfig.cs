namespace mood_tracker.Observability;

public static class LDConfig
{
    // Replace with your LaunchDarkly mobile key (Settings -> SDK keys).
    // This MUST be a mobile key, not a server-side SDK key.
    public const string MobileKey = "mob-48102222-6c44-4fdf-9331-44f6edb3beec";

    // How this app appears in LaunchDarkly observability.
    public const string ServiceName = "mood-tracker-maui-demo";

    // Feature flag keys. Create matching flags in your LaunchDarkly project.
    public const string UseBrokenEndpointFlag = "use-broken-endpoint";
    public const string CheckInFlowV2Flag     = "checkin-flow-v2";
}
