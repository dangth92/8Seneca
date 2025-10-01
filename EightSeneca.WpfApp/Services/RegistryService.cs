using Microsoft.Win32;
using System;

namespace EightSeneca.WpfApp.Services
{
    /// <summary>
    /// Service for reading and monitoring registry settings for web browser configuration
    /// </summary>
    public class RegistryService : IDisposable
    {
        private const string REGISTRY_PATH = @"Software\EightSeneca";
        private RegistryKey _registryKey;
        private bool _disposed = false;

        public RegistryService()
        {
            _registryKey = Registry.CurrentUser.OpenSubKey(REGISTRY_PATH);
        }

        /// <summary>
        /// Gets the browser URL from registry with fallback to default
        /// </summary>
        public string GetBrowserUrl()
        {
            return GetRegistryValue("BrowserUrl", "https://www.google.com");
        }

        /// <summary>
        /// Gets the web engine preference from registry
        /// </summary>
        public string GetWebEngine()
        {
            return GetRegistryValue("WebEngine", "WebView2");
        }

        /// <summary>
        /// Gets whether zoom is enabled from registry
        /// </summary>
        public bool GetEnableZoom()
        {
            return GetRegistryValue("EnableZoom", "true") == "true";
        }

        /// <summary>
        /// Gets whether touch events are enabled from registry
        /// </summary>
        public bool GetEnableTouch()
        {
            return GetRegistryValue("EnableTouch", "true") == "true";
        }

        /// <summary>
        /// Gets whether external links are allowed from registry
        /// </summary>
        public bool GetAllowExternalLinks()
        {
            return GetRegistryValue("AllowExternalLinks", "false") == "true";
        }

        /// <summary>
        /// Gets video file paths from registry
        /// </summary>
        public string GetVideoPath(int videoIndex)
        {
            if (videoIndex < 1 || videoIndex > 3)
                throw new ArgumentException("Video index must be 1, 2, or 3");

            return GetRegistryValue($"Video{videoIndex}Path", $@"C:\Videos\video{videoIndex}.mp4");
        }

        /// <summary>
        /// Generic method to read registry values with fallback
        /// </summary>
        private string GetRegistryValue(string valueName, string defaultValue)
        {
            try
            {
                return _registryKey?.GetValue(valueName, defaultValue) as string ?? defaultValue;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error reading registry value {valueName}: {ex.Message}");
                return defaultValue;
            }
        }

        public void Dispose()
        {
            if (!_disposed)
            {
                _registryKey?.Dispose();
                _disposed = true;
            }
        }
    }
}