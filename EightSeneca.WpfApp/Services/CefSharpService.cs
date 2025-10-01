using System;
using System.Threading.Tasks;
using System.Windows;
using CefSharp;
using CefSharp.Wpf;

namespace EightSeneca.WpfApp.Services
{
    public class CefSharpService : IWebBrowserService
    {
        private ChromiumWebBrowser _browser;
        private string _currentUrl;
        private bool _isInitialized = false;

        public object BrowserControl => _browser;

        public bool EnableZoom { get; set; } = true;
        public bool EnableTouch { get; set; } = true;
        public bool AllowExternalLinks { get; set; } = false;

        public event EventHandler<ExternalNavigationEventArgs> ExternalNavigationRequested;

        public async Task InitializeAsync()
        {
            await Application.Current.Dispatcher.InvokeAsync(() =>
            {
                try
                {
                    if (!Cef.IsInitialized.GetValueOrDefault())
                    {
                        var settings = new CefSettings
                        {
                            WindowlessRenderingEnabled = true,
                            LogSeverity = LogSeverity.Verbose,
                            MultiThreadedMessageLoop = false,
                            PersistSessionCookies = false,
                            //CachePath = "C:\\eightSeneca\\Cache",
                            //LogFile = "C:\\eightSeneca\\Debug.log",
                        };

                        settings.CefCommandLineArgs.Add("disable-gpu", "1");
                        settings.CefCommandLineArgs.Add("disable-gpu-compositing", "1");
                        settings.CefCommandLineArgs.Add("disable-gpu-vsync", "1");
                        settings.CefCommandLineArgs.Add("enable-begin-frame-scheduling", "1");
                        settings.CefCommandLineArgs.Add("disable-webgl", "1");
                        // Enable software rendering
                        settings.CefCommandLineArgs.Add("enable-software-rasterizer", "1");
                        settings.CefCommandLineArgs.Add("disable-direct-write", "1");

                        Cef.Initialize(settings, performDependencyCheck: true, browserProcessHandler: null);
                    }

                    _browser = new ChromiumWebBrowser();
                    ConfigureExternalLinkBlocking();
                    _isInitialized = true;
                }
                catch (Exception ex)
                {

                    throw ex;
                }

            });
        }

        public void Navigate(string url)
        {
            if (string.IsNullOrEmpty(url)) return;

            Application.Current.Dispatcher.Invoke(() =>
            {
                _currentUrl = url;
                _browser.Address = url;
            });
        }

        public void Dispose()
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                _browser?.Dispose();
            });
        }

        private void ConfigureExternalLinkBlocking()
        {
            _browser.LoadingStateChanged += (sender, e) =>
            {
                if (!AllowExternalLinks && e.IsLoading)
                {
                    var url = _browser.Address;
                    if (!string.IsNullOrEmpty(url) && IsExternalNavigation(url))
                    {
                        _browser.Stop();
                    }
                }
            };
        }

        private bool IsExternalNavigation(string navigatingUrl)
        {
            try
            {
                if (string.IsNullOrEmpty(_currentUrl)) return false;

                var currentUri = new Uri(_currentUrl);
                var navigatingUri = new Uri(navigatingUrl);

                return currentUri.Host != navigatingUri.Host;
            }
            catch
            {
                return true;
            }
        }
    }
}