using CefSharp;
using CefSharp.Wpf;
using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

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
                    bool isCefInitialized = true;
                    if (!Cef.IsInitialized.GetValueOrDefault())
                    {
                        isCefInitialized = InitCefSharp();
                    }

                    if (!Cef.IsInitialized.GetValueOrDefault() || !isCefInitialized)
                    {
                        return;
                    }

                    _browser = new ChromiumWebBrowser();

                    ConfigureCefSharpBrowserSettings();   

                    _isInitialized = true;
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Exception: {ex.Message}");
                }

            });
        }

        public void Navigate(string url)
        {
            if (string.IsNullOrEmpty(url) || !_isInitialized) 
                return;

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

        private void ConfigureCefSharpBrowserSettings()
        {
            if (_browser == null)
                return;

            if (!EnableZoom)
            {
                _browser.PreviewMouseWheel += (s, e) =>
                {
                    if (Keyboard.Modifiers == ModifierKeys.Control)
                    {
                        e.Handled = true; // Prevent Ctrl + Wheel
                    }
                };
            }

            ConfigureExternalLinkBlocking();
        }

        private void ConfigureExternalLinkBlocking()
        {
            if (_browser == null)
                return;

            _browser.RequestHandler = new CustomRequestHandler(this);
        }

        public bool IsExternalNavigation(string navigatingUrl)
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

        private bool InitCefSharp()
        {
            var cachePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "CefSharp\\Cache");
#if ANYCPU
            //Only required for PlatformTarget of AnyCPU
            CefRuntime.SubscribeAnyCpuAssemblyResolver();
#endif
            var settings = new CefSettings()
            {
                //By default CefSharp will use an in-memory cache, you need to specify a Cache Folder to persist data
                LogSeverity = LogSeverity.Warning, // chỉ log cảnh báo và lỗi
                LogFile = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "cef.log"),
                CachePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "cache"),
                BrowserSubprocessPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "CefSharp.BrowserSubprocess.exe")
            };

            //Example of setting a command line argument
            //Enables WebRTC
            // - CEF Doesn't currently support permissions on a per browser basis see https://bitbucket.org/chromiumembedded/cef/issues/2582/allow-run-time-handling-of-media-access
            // - CEF Doesn't currently support displaying a UI for media access permissions
            //
            //NOTE: WebRTC Device Id's aren't persisted as they are in Chrome see https://bitbucket.org/chromiumembedded/cef/issues/2064/persist-webrtc-deviceids-across-restart
            settings.CefCommandLineArgs.Add("enable-media-stream");
            //https://peter.sh/experiments/chromium-command-line-switches/#use-fake-ui-for-media-stream
            settings.CefCommandLineArgs.Add("use-fake-ui-for-media-stream");
            //For screen sharing add (see https://bitbucket.org/chromiumembedded/cef/issues/2582/allow-run-time-handling-of-media-access#comment-58677180)
            settings.CefCommandLineArgs.Add("enable-usermedia-screen-capturing");

            settings.CefCommandLineArgs.Add("enable-logging");
            settings.CefCommandLineArgs.Add("v", "1");

            // Prevent Cef.IsInitialized = false
            settings.CefCommandLineArgs.Add("disable-sync", "1");
            settings.CefCommandLineArgs.Add("disable-background-networking", "1");
            settings.CefCommandLineArgs.Add("disable-default-apps", "1");
            settings.CefCommandLineArgs.Add("disable-component-update", "1");
            settings.CefCommandLineArgs.Add("disable-client-side-phishing-detection", "1");
            settings.CefCommandLineArgs.Add("disable-extensions", "1");
            settings.CefCommandLineArgs.Add("disable-background-mode", "1");
            settings.CefCommandLineArgs.Add("disable-features", "AccountConsistency,PushMessaging,Notification,BackgroundFetch,BackgroundSync");
            settings.CefCommandLineArgs.Add("no-default-browser-check", "1");
            settings.CefCommandLineArgs.Add("no-first-run", "1");
            settings.CefCommandLineArgs.Add("disable-ipc-flooding-protection", "1");
            settings.CefCommandLineArgs.Add("disable-prompt-on-repost", "1");
            settings.CefCommandLineArgs.Add("disable-renderer-backgrounding", "1");
            settings.CefCommandLineArgs.Add("disable-background-timer-throttling", "1");
            settings.CefCommandLineArgs.Add("disable-domain-reliability", "1");
            settings.CefCommandLineArgs.Add("disable-notifications", "1");
            settings.CefCommandLineArgs.Add("disable-permissions-api", "1");
            settings.CefCommandLineArgs.Add("disable-remote-fonts", "1");
            settings.CefCommandLineArgs.Add("no-service-autorun", "1");

            // Enable zoom, touch gestures
            settings.CefCommandLineArgs.Add("enable-pinch", EnableZoom ? "1" : "0");
            //settings.CefCommandLineArgs.Add("disable-pinch", EnableZoom ? "false" : "true");
            
            if (EnableTouch)
            {
                settings.CefCommandLineArgs.Add("touch-events", "enabled");
                settings.CefCommandLineArgs.Add("enable-features", "TouchpadAndWheelScrollLatching,AsyncWheelEvents");
                settings.CefCommandLineArgs.Add("touch-selection-strategy", "character");
            }
            else
            {
                settings.CefCommandLineArgs.Add("touch-events", "disabled");
                settings.CefCommandLineArgs.Add("disable-touch-adjustment", "1");
            }

            //Example of checking if a call to Cef.Initialize has already been made, we require this for
            //our .Net 5.0 Single File Publish example, you don't typically need to perform this check
            //if you call Cef.Initialze within your WPF App constructor.
            if (Cef.IsInitialized == null)
            {
                //Perform dependency check to make sure all relevant resources are in our output directory.
                var initialized = Cef.Initialize(settings, performDependencyCheck: true, browserProcessHandler: null);

                if (!initialized)
                {
                    var exitCode = Cef.GetExitCode();

                    if (exitCode == CefSharp.Enums.ResultCode.NormalExitProcessNotified)
                    {
                        MessageBox.Show($"Cef.Initialize failed with {exitCode}, another process is already using cache path {cachePath}");
                    }
                    else
                    {
                        MessageBox.Show($"Cef.Initialize failed with {exitCode}, check the log file for more details.");
                    }
                }

                return initialized;
            }

            return true;
        }
    }
}