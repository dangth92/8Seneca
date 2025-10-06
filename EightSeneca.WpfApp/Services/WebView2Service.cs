using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using Microsoft.Web.WebView2.Core;
using Microsoft.Web.WebView2.Wpf;

namespace EightSeneca.WpfApp.Services
{
    public class WebView2Service : IWebBrowserService
    {
        private WebView2 _webView;
        private bool _enableZoom = true;
        private bool _enableTouch = true;
        private bool _allowExternalLinks = false;
        private bool _isInitialized = false;
        private string _currentUrl;

        public object BrowserControl => _webView;

        public bool EnableZoom
        {
            get => _enableZoom;
            set
            {
                _enableZoom = value;
                UpdateZoomSettings();
            }
        }

        public bool EnableTouch
        {
            get => _enableTouch;
            set => _enableTouch = value;
        }

        public bool AllowExternalLinks
        {
            get => _allowExternalLinks;
            set => _allowExternalLinks = value;
        }

        public event EventHandler<ExternalNavigationEventArgs> ExternalNavigationRequested;

        public WebView2Service()
        {
            _webView = new WebView2();
        }

        /// <summary>
        /// Initializes the WebView2 control with the simple activation trick
        /// </summary>
        public async Task InitializeAsync()
        {
            try
            {
                System.Diagnostics.Debug.WriteLine("Starting WebView2 initialization with activation trick...");

                // Simple check for WebView2 runtime
                try
                {
                    var version = CoreWebView2Environment.GetAvailableBrowserVersionString();
                    if (string.IsNullOrEmpty(version))
                    {
                        ShowErrorMessage("WebView2 Runtime not found");
                        return;
                    }
                }
                catch
                {
                    ShowErrorMessage("WebView2 Runtime not available");
                    return;
                }

                // Create environment
                var environment = await CoreWebView2Environment.CreateAsync();

                // APPLY THE FIX: Temporarily activate and deactivate the control
                await ActivateWebViewTrick();

                // Now initialize - this should work after the activation trick
                await _webView.EnsureCoreWebView2Async(environment);

                if (_webView.CoreWebView2 != null)
                {
                    ConfigureWebView2Settings();
                    _isInitialized = true;
                    System.Diagnostics.Debug.WriteLine("WebView2 initialized successfully with activation trick");
                }
                else
                {
                    ShowErrorMessage("WebView2 initialization failed");
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"WebView2 initialization failed: {ex.Message}");
                ShowErrorMessage($"WebView2 initialization failed: {ex.Message}");
            }
        }

        /// <summary>
        /// THE ACTUAL FIX: Temporarily activate and deactivate the WebView2 control
        /// This forces the WebView2 to properly initialize by briefly making it "active"
        /// </summary>
        private async Task ActivateWebViewTrick()
        {
            try
            {
                // Ensure we're on the UI thread
                await _webView.Dispatcher.InvokeAsync(() =>
                {
                    // Store current state
                    var currentVisibility = _webView.Visibility;

                    // HACK: Briefly make the WebView visible and active to force initialization
                    _webView.Visibility = Visibility.Visible;
                    _webView.IsEnabled = true;

                    // Force layout update
                    _webView.UpdateLayout();

                    // IMMEDIATELY revert back (this is the key trick)
                    _webView.Visibility = currentVisibility;

                    System.Diagnostics.Debug.WriteLine("WebView2 activation trick applied");
                });

                // Small delay to let the activation take effect
                await Task.Delay(100);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"WebView2 activation trick failed: {ex.Message}");
            }
        }

        private void ConfigureWebView2Settings()
        {
            if (_webView.CoreWebView2?.Settings != null)
            {
                _webView.CoreWebView2.Settings.IsZoomControlEnabled = _enableZoom;
                _webView.CoreWebView2.Settings.IsPinchZoomEnabled = _enableZoom;
                _webView.CoreWebView2.Settings.IsStatusBarEnabled = false;
                _webView.CoreWebView2.Settings.AreDefaultContextMenusEnabled = true;

                ConfigureExternalLinkBlocking();
            }
        }

        private void ConfigureExternalLinkBlocking()
        {
            _webView.CoreWebView2.NavigationStarting += (sender, e) =>
            {
                if (!_allowExternalLinks && IsExternalNavigation(e.Uri))
                {
                    e.Cancel = true;
                }
            };
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

        private void ShowErrorMessage(string message)
        {
            try
            {
                _webView.Dispatcher.Invoke(() =>
                {
                    if (_webView.CoreWebView2 != null)
                    {
                        _webView.CoreWebView2.NavigateToString($"<h1>Error: {message}</h1>");
                    }
                });
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error showing error message: {ex.Message}");
            }
        }

        private void UpdateZoomSettings()
        {
            if (_webView.CoreWebView2?.Settings != null)
            {
                _webView.CoreWebView2.Settings.IsZoomControlEnabled = _enableZoom;
                _webView.CoreWebView2.Settings.IsPinchZoomEnabled = _enableZoom;
            }
        }

        public void Navigate(string url)
        {
            if (string.IsNullOrEmpty(url)) return;

            if (_isInitialized && _webView.CoreWebView2 != null)
            {
                try
                {
                    _webView.Dispatcher.Invoke(() =>
                    {
                        _currentUrl = url;
                        _webView.Source = new Uri(url);
                    });
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"Navigation error: {ex.Message}");
                }
            }
        }

        public void Dispose()
        {
            _webView?.Dispose();
        }
    }
}