using Microsoft.Win32;

namespace EightSeneca.RegistryTool.Models
{
    public static class RegistryConfig
    {
        public const string REGISTRY_BASE_PATH = @"Software\EightSeneca";

        public static readonly RegistryKeyDefinition[] Keys = new[]
        {
            new RegistryKeyDefinition("BrowserUrl", "https://www.google.com", RegistryValueKind.String),
            new RegistryKeyDefinition("WebEngine", "WebView2", RegistryValueKind.String), // "WebView2" hoặc "CefSharp"
            new RegistryKeyDefinition("EnableZoom", "true", RegistryValueKind.String), // "true" hoặc "false"
            new RegistryKeyDefinition("EnableTouch", "true", RegistryValueKind.String),
            new RegistryKeyDefinition("AllowExternalLinks", "false", RegistryValueKind.String),
            new RegistryKeyDefinition("Video1Path", @"C:\Videos\video1.mp4", RegistryValueKind.String),
            new RegistryKeyDefinition("Video2Path", @"C:\Videos\video2.mp4", RegistryValueKind.String),
            new RegistryKeyDefinition("Video3Path", @"C:\Videos\video3.mp4", RegistryValueKind.String)
        };
    }

    public class RegistryKeyDefinition
    {
        public string Name { get; }
        public object DefaultValue { get; }
        public RegistryValueKind ValueKind { get; }

        public RegistryKeyDefinition(string name, object defaultValue, RegistryValueKind valueKind)
        {
            Name = name;
            DefaultValue = defaultValue;
            ValueKind = valueKind;
        }
    }
}