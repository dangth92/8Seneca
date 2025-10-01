namespace RegistryTool.Models;
public static class AppRegistryKeys
{
    // Base registry path cho 8Seneca test
    public const string BasePath = @"Software\8SenecaWpfTask";

    // Keys
    public const string UrlKey = "Url";
    public const string EngineKey = "Engine";
    public const string ZoomEnabledKey = "ZoomEnabled";
    public const string TouchEnabledKey = "TouchEnabled";
    public const string FollowExternalLinkKey = "FollowExternalLink";

    // Default values
    public static readonly string DefaultUrl = "https://cnn.com";
    public static readonly string DefaultEngine = "WebView2";
    public const bool DefaultZoomEnabled = true;
    public const bool DefaultTouchEnabled = false;
    public const bool DefaultFollowExternalLink = false;
}