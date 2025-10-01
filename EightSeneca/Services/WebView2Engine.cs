using EightSeneca.Contracts;
using Microsoft.Web.WebView2.Core;
using Microsoft.Web.WebView2.Wpf;
using System.Windows;

namespace EightSeneca.Services;

public class WebView2Engine : IWebEngine
{
    private readonly WebView2 webView;
    private bool followExternalAllowed = false;

    public WebView2Engine()
    {
        webView = new WebView2
        {
            HorizontalAlignment = HorizontalAlignment.Stretch,
            VerticalAlignment = VerticalAlignment.Stretch
        };
        webView.NavigationStarting += WebView_NavigationStarting;
        webView.CoreWebView2InitializationCompleted += WebView_CoreWebView2InitializationCompleted;
    }

    public FrameworkElement GetView() => webView;

    public async void Initialize()
    {
        try
        {
            // initialize environment (async)
            var env = await CoreWebView2Environment.CreateAsync();
            await webView.EnsureCoreWebView2Async(env);
            // default settings
            webView.CoreWebView2.Settings.IsScriptEnabled = true;
            webView.CoreWebView2.Settings.AreDefaultContextMenusEnabled = false;
            webView.CoreWebView2.Settings.IsZoomControlEnabled = true;
            webView.CoreWebView2.Settings.AreDevToolsEnabled = false;
        }
        catch (Exception ex)
        {
            // handle (log) but don't crash
            Console.WriteLine("WebView2 initialize failed: " + ex.Message);
        }
    }

    private void WebView_CoreWebView2InitializationCompleted(object? sender, CoreWebView2InitializationCompletedEventArgs e)
    {
        // After initialization, you can set more
    }

    public void Navigate(string url)
    {
        if (webView.CoreWebView2 != null)
        {
            try
            {
                webView.CoreWebView2.Navigate(url);
            }
            catch
            {
                webView.Source = new Uri("about:blank");
            }
        }
        else
        {
            // fallback: set Source (will navigate after core ready)
            try { webView.Source = new Uri(url); } catch { webView.Source = new Uri("about:blank"); }
        }
    }

    public void SetZoomEnabled(bool enabled)
    {
        // WebView2 supports zoom via ZoomFactor. We'll allow pinch/ctrl+scroll if enabled; complex gestures require JS injection.
        if (webView.CoreWebView2 != null)
        {
            webView.CoreWebView2.Settings.IsZoomControlEnabled = enabled;
        }
    }

    public void SetTouchEnabled(bool enabled)
    {
        // No direct toggle; use CSS/JS to disable pointer events if needed.
        if (webView.CoreWebView2 != null)
        {
            var script = enabled
                ? "document.documentElement.style.pointerEvents = 'auto';"
                : "document.documentElement.style.pointerEvents = 'none';";
            _ = webView.CoreWebView2.ExecuteScriptAsync(script);
        }
    }

    public void SetFollowExternalLinks(bool allowed)
    {
        followExternalAllowed = allowed;
    }

    private void WebView_NavigationStarting(object? sender, CoreWebView2NavigationStartingEventArgs e)
    {
        if (!followExternalAllowed)
        {
            // compare hosts: allow if same host as current document
            try
            {
                var dest = new Uri(e.Uri);
                Uri? current = null;
                if (webView.Source != null) current = webView.Source;
                else if (webView.CoreWebView2?.DocumentTitle != null)
                {
                    current = new Uri(webView.CoreWebView2.Source ?? "about:blank");
                }
                if (current != null)
                {
                    if (!string.Equals(current.Host, dest.Host, StringComparison.OrdinalIgnoreCase))
                    {
                        // cancel external navigation
                        e.Cancel = true;
                    }
                }
                // If current unknown, allow navigation
            }
            catch
            {
                // if any parse error, allow
            }
        }
    }
}
