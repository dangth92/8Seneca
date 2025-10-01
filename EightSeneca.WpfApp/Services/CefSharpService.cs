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

        public object BrowserControl => _browser;

        public bool EnableZoom { get; set; } = true;
        public bool EnableTouch { get; set; } = true;
        public bool AllowExternalLinks { get; set; } = false;

        public event EventHandler<ExternalNavigationEventArgs> ExternalNavigationRequested;

        public async Task InitializeAsync()
        {
            var settings02 = new CefSettings()
            {
                CachePath = "C:\\Cache",
                LogFile = "C:\\Debug.log",
                LogSeverity = LogSeverity.Default
            };
            //Perform dependency check to make sure all relevant resources are in our output directory.
            Cef.Initialize(settings02, performDependencyCheck: true, browserProcessHandler: null);

            _browser = new ChromiumWebBrowser();

            await Application.Current.Dispatcher.InvokeAsync(() =>
            {
                try
                {
                    if (!Cef.IsInitialized.GetValueOrDefault())
                    {
                        //Set the cache path
                        var settings = new CefSettings() 
                        { 
                            CachePath = "C:\\Cache",
                            LogFile = "C:\\Debug.log", 
                            LogSeverity = LogSeverity.Default
                        };
                        //Perform dependency check to make sure all relevant resources are in our output directory.
                        Cef.Initialize(settings, performDependencyCheck: true, browserProcessHandler: null);

                        //var settings = new CefSettings()
                        //{
                        //    LogFile = "C:\\Debug.log", //You can customise this path
                        //    LogSeverity = LogSeverity.Warning // You can change the log level
                        //};

                        //Cef.Initialize(settings);
                    }

                    _browser = new ChromiumWebBrowser();
                    ConfigureExternalLinkBlocking();
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