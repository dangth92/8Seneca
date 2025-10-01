using Microsoft.Web.WebView2.Core;
using Microsoft.Web.WebView2.Wpf;
using System;
using System.Windows;

namespace EightSeneca.Wpf.Services
{
    public class WebView2Engine : IWebEngine
    {
        private readonly WebView2 _view;
        private bool _followAllowed = false;

        public WebView2Engine()
        {
            _view = new WebView2();
            _view.NavigationStarting += Core_NavigationStarting;
        }

        public FrameworkElement GetView() => _view;

        public async void Initialize()
        {
            try
            {
                var env = await CoreWebView2Environment.CreateAsync();
                await _view.EnsureCoreWebView2Async(env);
                // default settings
                _view.CoreWebView2.Settings.IsScriptEnabled = true;
                _view.CoreWebView2.Settings.IsStatusBarEnabled = false;
            }
            catch (Exception ex) { Console.WriteLine("WebView2 init error: " + ex.Message); }
        }

        public void Navigate(string url)
        {
            if (Uri.TryCreate(url, UriKind.Absolute, out var u))
            {
                if (_view.CoreWebView2 != null) _view.CoreWebView2.Navigate(u.ToString());
                else _view.Source = u;
            }
        }

        public void SetZoomEnabled(bool enabled)
        {
            if (_view.CoreWebView2 != null)
                _view.CoreWebView2.Settings.IsZoomControlEnabled = enabled;
        }

        public void SetTouchEnabled(bool enabled)
        {
            if (_view.CoreWebView2 != null)
            {
                var script = enabled ? "document.documentElement.style.pointerEvents='auto';" : "document.documentElement.style.pointerEvents='none';";
                _view.CoreWebView2.ExecuteScriptAsync(script);
            }
        }

        public void SetFollowExternalLinks(bool allowed) => _followAllowed = allowed;

        private void Core_NavigationStarting(object sender, CoreWebView2NavigationStartingEventArgs e)
        {
            if (_followAllowed) return;
            try
            {
                var dest = new Uri(e.Uri);
                var cur = _view.Source;
                if (cur != null && !string.IsNullOrEmpty(cur.Host))
                {
                    if (!string.Equals(cur.Host, dest.Host, StringComparison.OrdinalIgnoreCase))
                        e.Cancel = true;
                }
            }
            catch { }
        }
    }
}
