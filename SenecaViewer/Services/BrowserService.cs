using System;
using System.Windows;

namespace SenecaViewer.Services
{
    public class BrowserService
    {
        public UIElement CreateBrowser(string engine, string url, bool enableZoom, bool allowExternalLinks)
        {
            if (engine.Equals("CefSharp", StringComparison.OrdinalIgnoreCase))
            {
                return CreateCefSharp(url, enableZoom, allowExternalLinks);
            }
            else
            {
                return CreateWebView2(url, enableZoom, allowExternalLinks);
            }
        }

        private UIElement CreateWebView2(string url, bool enableZoom, bool allowExternalLinks)
        {
            var webView = new Microsoft.Web.WebView2.Wpf.WebView2();
            webView.Source = new Uri(url);

            webView.CoreWebView2InitializationCompleted += (s, e) =>
            {
                if (e.IsSuccess && webView.CoreWebView2 != null)
                {
                    var settings = webView.CoreWebView2.Settings;
                    settings.AreDefaultContextMenusEnabled = true;
                    settings.IsZoomControlEnabled = enableZoom;

                    if (!allowExternalLinks)
                    {
                        Uri sourceUri = new Uri(url);
                        webView.CoreWebView2.NavigationStarting += (sender, args) =>
                        {
                            try
                            {
                                Uri requestUri = new Uri(args.Uri);
                                if (requestUri.Host != sourceUri.Host)
                                {
                                    args.Cancel = true;
                                }
                            }
                            catch
                            {
                                args.Cancel = true;
                            }
                        };
                    }
                }
            };

            webView.EnsureCoreWebView2Async();
            return webView;
        }

        private UIElement CreateCefSharp(string url, bool enableZoom, bool allowExternalLinks)
        {
            var chromium = new CefSharp.Wpf.ChromiumWebBrowser();
            chromium.Address = url;

            if (!allowExternalLinks)
            {
                chromium.RequestHandler = new CustomRequestHandler(url);
            }

            return chromium;
        }

        public void NavigateToUrl(UIElement browser, string url)
        {
            if (browser is Microsoft.Web.WebView2.Wpf.WebView2 webView)
            {
                webView.Source = new Uri(url);
            }
            else if (browser is CefSharp.Wpf.ChromiumWebBrowser chromium)
            {
                chromium.Address = url;
            }
        }
    }

    public class CustomRequestHandler : CefSharp.Handler.RequestHandler
    {
        private readonly string _baseUrl;

        public CustomRequestHandler(string baseUrl)
        {
            _baseUrl = baseUrl;
        }

        protected override bool OnBeforeBrowse(CefSharp.IWebBrowser chromiumWebBrowser, CefSharp.IBrowser browser,
            CefSharp.IFrame frame, CefSharp.IRequest request, bool userGesture, bool isRedirect)
        {
            try
            {
                Uri requestUri = new Uri(request.Url);
                Uri sourceUri = new Uri(_baseUrl);

                if (requestUri.Host != sourceUri.Host)
                {
                    return true; // Cancel navigation
                }
            }
            catch
            {
                return true;
            }

            return base.OnBeforeBrowse(chromiumWebBrowser, browser, frame, request, userGesture, isRedirect);
        }
    }
}