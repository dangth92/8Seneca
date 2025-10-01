using Microsoft.Win32;
using System;
using System.Threading;

namespace SenecaViewer.Services
{
    public class RegistryService : IDisposable
    {
        private const string REGISTRY_PATH = @"Software\8Seneca";
        private Timer _monitorTimer;
        private string _lastUrl = string.Empty;

        public event EventHandler<string> UrlChanged;

        public string GetBrowserUrl()
        {
            return GetStringValue("BrowserUrl", "https://www.google.com");
        }

        public string GetWebEngine()
        {
            return GetStringValue("WebEngine", "WebView2");
        }

        public bool GetEnableZoom()
        {
            return GetBoolValue("EnableZoom", true);
        }

        public bool GetEnableTouch()
        {
            return GetBoolValue("EnableTouch", true);
        }

        public bool GetAllowExternalLinks()
        {
            return GetBoolValue("AllowExternalLinks", false);
        }

        public string GetVideoPath(int index)
        {
            return GetStringValue($"Video{index}Path", $"C:\\Videos\\video{index}.mp4");
        }

        private string GetStringValue(string key, string defaultValue)
        {
            try
            {
                using (var regKey = Registry.CurrentUser.OpenSubKey(REGISTRY_PATH))
                {
                    if (regKey == null) return defaultValue;
                    var value = regKey.GetValue(key);
                    return value?.ToString() ?? defaultValue;
                }
            }
            catch
            {
                return defaultValue;
            }
        }

        private bool GetBoolValue(string key, bool defaultValue)
        {
            string strValue = GetStringValue(key, defaultValue.ToString().ToLower());
            return strValue.ToLower() == "true";
        }

        public void StartMonitoring()
        {
            _lastUrl = GetBrowserUrl();
            _monitorTimer = new Timer(CheckRegistryChanges, null, 0, 1000);
        }

        private void CheckRegistryChanges(object state)
        {
            try
            {
                string currentUrl = GetBrowserUrl();
                if (currentUrl != _lastUrl)
                {
                    _lastUrl = currentUrl;
                    UrlChanged?.Invoke(this, currentUrl);
                }
            }
            catch
            {
                // Ignore monitoring errors
            }
        }

        public void StopMonitoring()
        {
            _monitorTimer?.Dispose();
            _monitorTimer = null;
        }

        public void Dispose()
        {
            StopMonitoring();
        }
    }
}