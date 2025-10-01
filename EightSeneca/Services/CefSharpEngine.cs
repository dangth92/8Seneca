using CefSharp.Wpf;
using EightSeneca.Contracts;
using System.Windows;
using CefSharp;
using HorizontalAlignment = System.Windows.HorizontalAlignment;

namespace EightSeneca.Services
{
    public class CefSharpEngine : IWebEngine
    {
        private ChromiumWebBrowser browser;
        private bool followExternalAllowed = false;

        public CefSharpEngine()
        {
            browser = new ChromiumWebBrowser
            {
                HorizontalAlignment = HorizontalAlignment.Stretch,
                VerticalAlignment = VerticalAlignment.Stretch
            };

            // register event
            browser.FrameLoadStart += Browser_FrameLoadStart;
            browser.AddressChanged += Browser_AddressChanged;
            // Note: Cef.Initialize must be called once in app startup (App.OnStartup)
        }

        public FrameworkElement GetView() => browser;

        public void Initialize()
        {
            // ensure Cef is initialized (call once)
            if (!Cef.IsInitialized.GetValueOrDefault())
            {
                var settings = new CefSettings()
                {
                    // you can set CachePath, LogSeverity etc.
                };
                Cef.Initialize(settings, performDependencyCheck: true, browserProcessHandler: null);
            }
        }

        public void Navigate(string url) => browser.Load(url);

        public void SetZoomEnabled(bool enabled)
        {
            // CefSharp supports zoom via browser.SetZoomLevel
            // We won't toggle UI-level zoom gestures here; you can manage zoom commands later.
        }

        public void SetTouchEnabled(bool enabled)
        {
            // touch support is complex; can inject JS to disable pointer events similar to WebView2
            var script = enabled ? "document.documentElement.style.pointerEvents='auto';" : "document.documentElement.style.pointerEvents='none';";
            browser.GetMainFrame()?.ExecuteJavaScriptAsync(script);
        }

        public void SetFollowExternalLinks(bool allowed)
        {
            followExternalAllowed = allowed;
        }

        private void Browser_FrameLoadStart(object? sender, FrameLoadStartEventArgs e)
        {
            // Not used for blocking; use OnBeforeBrowse handler via IRequestHandler for fine control.
        }

        private void Browser_AddressChanged(object? sender, DependencyPropertyChangedEventArgs e)
        {
            // address changed
        }
    }
}
