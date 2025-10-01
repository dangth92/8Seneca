using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Timers;
using System.Threading.Tasks;
using CefSharp;

namespace EightSeneca.WpfApp.Services
{
    /// <summary>
    /// Main web browser service that coordinates between WebView2 and CefSharp engines
    /// Acts as a factory and facade for the selected web browser engine
    /// Implements registry-based engine selection and URL monitoring
    /// </summary>
    public class WebBrowserService : INotifyPropertyChanged, IDisposable
    {
        private readonly RegistryService _registryService;
        private IWebBrowserService _currentBrowser;
        private Timer _registryMonitorTimer;
        private string _lastKnownUrl;

        public object BrowserControl => _currentBrowser?.BrowserControl;

        public bool EnableZoom
        {
            get => _currentBrowser?.EnableZoom ?? true;
            set
            {
                if (_currentBrowser != null)
                    _currentBrowser.EnableZoom = value;
            }
        }

        public bool EnableTouch
        {
            get => _currentBrowser?.EnableTouch ?? true;
            set
            {
                if (_currentBrowser != null)
                    _currentBrowser.EnableTouch = value;
            }
        }

        public bool AllowExternalLinks
        {
            get => _currentBrowser?.AllowExternalLinks ?? false;
            set
            {
                if (_currentBrowser != null)
                    _currentBrowser.AllowExternalLinks = value;
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public WebBrowserService(RegistryService registryService)
        {
            _registryService = registryService;
        }

        /// <summary>
        /// Initializes the web browser service with registry settings ASYNCHRONOUSLY
        /// </summary>
        public async Task InitializeAsync()
        {
            try
            {
                // Get settings from registry
                var webEngine = _registryService.GetWebEngine();
                var browserUrl = _registryService.GetBrowserUrl();
                var enableZoom = _registryService.GetEnableZoom();
                var enableTouch = _registryService.GetEnableTouch();
                var allowExternalLinks = _registryService.GetAllowExternalLinks();

                System.Diagnostics.Debug.WriteLine($"Initializing web browser with engine: {webEngine}, URL: {browserUrl}");

                // Create and initialize the selected web engine
                _currentBrowser = CreateWebBrowser(webEngine);

                if (_currentBrowser != null)
                {
                    // Configure browser settings
                    _currentBrowser.EnableZoom = enableZoom;
                    _currentBrowser.EnableTouch = enableTouch;
                    _currentBrowser.AllowExternalLinks = allowExternalLinks;

                    // AWAIT the asynchronous initialization
                    System.Diagnostics.Debug.WriteLine("Starting browser initialization...");
                    await _currentBrowser.InitializeAsync();
                    System.Diagnostics.Debug.WriteLine("Browser initialization completed");

                    // Now navigate to initial URL - browser should be ready
                    _currentBrowser.Navigate(browserUrl);
                    _lastKnownUrl = browserUrl;

                    // Start monitoring registry for changes
                    StartRegistryMonitoring();

                    System.Diagnostics.Debug.WriteLine($"Web browser initialized with engine: {webEngine}");

                    // Notify UI that browser control is ready
                    OnPropertyChanged(nameof(BrowserControl));
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Web browser service initialization failed: {ex.Message}");
                await CreateFallbackBrowserAsync();
            }
        }

        /// <summary>
        /// Factory method to create the appropriate web browser engine based on registry setting
        /// </summary>
        private IWebBrowserService CreateWebBrowser(string webEngine)
        {
            try
            {
                switch (webEngine?.ToLower())
                {
                    case "cefsharp":
                        // Additional check for CefSharp availability
                        if (!Cef.IsInitialized.GetValueOrDefault())
                        {
                            return new WebView2Service();
                        }

                        return new CefSharpService();

                    case "webview2":
                    default:
                        return new WebView2Service();
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Failed to create {webEngine} browser, falling back to WebView2: {ex.Message}");
                return new WebView2Service();
            }
        }

        /// <summary>
        /// Creates a fallback browser when the selected engine fails to initialize
        /// </summary>
        private async Task CreateFallbackBrowserAsync()
        {
            try
            {
                System.Diagnostics.Debug.WriteLine("Creating fallback WebView2 browser");
                _currentBrowser = new WebView2Service();
                await _currentBrowser.InitializeAsync();

                var browserUrl = _registryService.GetBrowserUrl();
                _currentBrowser.Navigate(browserUrl);
                _lastKnownUrl = browserUrl;

                OnPropertyChanged(nameof(BrowserControl));
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Fallback browser creation failed: {ex.Message}");
            }
        }

        /// <summary>
        /// Starts monitoring registry for URL changes
        /// </summary>
        private void StartRegistryMonitoring()
        {
            _registryMonitorTimer = new Timer(2000); // Check every 2 seconds
            _registryMonitorTimer.Elapsed += OnRegistryCheckTimerElapsed;
            _registryMonitorTimer.Start();

            System.Diagnostics.Debug.WriteLine("Registry URL monitoring started");
        }

        /// <summary>
        /// Timer event handler to check for registry URL changes
        /// </summary>
        private void OnRegistryCheckTimerElapsed(object sender, ElapsedEventArgs e)
        {
            try
            {
                var currentUrl = _registryService.GetBrowserUrl();
                System.Diagnostics.Debug.WriteLine($"Registry check - Current URL in registry: {currentUrl}, Last known: {_lastKnownUrl}");

                // If URL has changed, navigate to the new URL
                if (currentUrl != _lastKnownUrl && !string.IsNullOrEmpty(currentUrl))
                {
                    System.Diagnostics.Debug.WriteLine($"Registry URL changed from '{_lastKnownUrl}' to '{currentUrl}'");
                    _lastKnownUrl = currentUrl;

                    // Use dispatcher to navigate on UI thread
                    System.Windows.Application.Current?.Dispatcher?.Invoke(() =>
                    {
                        System.Diagnostics.Debug.WriteLine($"Dispatching navigation to: {currentUrl}");
                        _currentBrowser?.Navigate(currentUrl);
                    });
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error checking registry for URL changes: {ex.Message}");
            }
        }

        /// <summary>
        /// Navigates to the specified URL using the current browser engine
        /// </summary>
        public void Navigate(string url)
        {
            _currentBrowser?.Navigate(url);
        }

        public void Dispose()
        {
            _registryMonitorTimer?.Stop();
            _registryMonitorTimer?.Dispose();
            _currentBrowser?.Dispose();
        }

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}