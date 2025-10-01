using CefSharp;
using CefSharp.Wpf;
using System;
using System.Security.Cryptography.X509Certificates;
using System.Windows;

namespace EightSeneca.Wpf.Services
{
    public class CefSharpEngine : IWebEngine
    {
        private ChromiumWebBrowser _browser;
        private bool _followAllowed = false;
        private string _currentHost = "";

        public CefSharpEngine()
        {
            _browser = new ChromiumWebBrowser();
            _browser.FrameLoadEnd += Browser_FrameLoadEnd;
            _browser.RequestHandler = new InternalRequestHandler(this);
        }

        public FrameworkElement GetView() => _browser;

        public void Initialize() { /* Cef initialized in App startup */ }

        public void Navigate(string url) => _browser.Load(url);

        public void SetZoomEnabled(bool enabled)
        {
            // implement as needed: can call _browser.SetZoomLevel
        }

        public void SetTouchEnabled(bool enabled)
        {
            // implement via JS injection
            var script = enabled ? "document.documentElement.style.pointerEvents='auto';" : "document.documentElement.style.pointerEvents='none';";
            _browser.ExecuteScriptAsync(script);
        }

        public void SetFollowExternalLinks(bool allowed) => _followAllowed = allowed;

        private void Browser_FrameLoadEnd(object sender, FrameLoadEndEventArgs e)
        {
            if (e.Frame.IsMain)
            {
                try { _currentHost = new Uri(_browser.Address ?? "about:blank").Host; } catch { _currentHost = ""; }
            }
        }

        private class InternalRequestHandler : IRequestHandler
        {
            private readonly CefSharpEngine _parent;
            public InternalRequestHandler(CefSharpEngine parent) { _parent = parent; }

            public bool OnBeforeBrowse(IWebBrowser browserControl, IBrowser browser, IFrame frame, IRequest request, bool isRedirect)
            {
                if (!_parent._followAllowed)
                {
                    try
                    {
                        var dest = new Uri(request.Url);
                        if (!string.IsNullOrEmpty(_parent._currentHost))
                        {
                            if (!string.Equals(_parent._currentHost, dest.Host, StringComparison.OrdinalIgnoreCase))
                                return true; // cancel navigation
                        }
                    }
                    catch { }
                }
                return false;
            }

            // ... other IRequestHandler methods: return default/do nothing
            public IResourceRequestHandler GetResourceRequestHandler(IWebBrowser chromiumWebBrowser, IBrowser browser, IFrame frame, IRequest request, bool isNavigation, bool isDownload, string requestInitiator, ref bool disableDefaultHandling) => null;
            public bool GetAuthCredentials(IWebBrowser chromiumWebBrowser, IBrowser browser, IFrame frame, bool isProxy, string host, int port, string realm, string scheme, IAuthCallback callback) => false;
            public bool OnCertificateError(IWebBrowser chromiumWebBrowser, IBrowser browser, CefErrorCode errorCode, string requestUrl, ISslInfo sslInfo, IRequestCallback callback) => false;
            public void OnPluginCrashed(IWebBrowser chromiumWebBrowser, IBrowser browser, string pluginPath) { }
            public bool OnSelectClientCertificate(IWebBrowser chromiumWebBrowser, IBrowser browser, bool isProxy, string host, int port, X509Certificate2Collection certificates, ISelectClientCertificateCallback callback) => false;
            public void OnRenderProcessTerminated(IWebBrowser chromiumWebBrowser, IBrowser browser, CefTerminationStatus status) { }
            public bool OnQuotaRequest(IWebBrowser chromiumWebBrowser, IBrowser browser, string originUrl, long newSize, IRequestCallback callback) => false;
            public void OnRenderViewReady(IWebBrowser chromiumWebBrowser, IBrowser browser) { }

            bool IRequestHandler.OnBeforeBrowse(IWebBrowser chromiumWebBrowser, IBrowser browser, IFrame frame, IRequest request, bool userGesture, bool isRedirect)
            {
                throw new NotImplementedException();
            }

            void IRequestHandler.OnDocumentAvailableInMainFrame(IWebBrowser chromiumWebBrowser, IBrowser browser)
            {
                throw new NotImplementedException();
            }

            bool IRequestHandler.OnOpenUrlFromTab(IWebBrowser chromiumWebBrowser, IBrowser browser, IFrame frame, string targetUrl, WindowOpenDisposition targetDisposition, bool userGesture)
            {
                throw new NotImplementedException();
            }

            bool IRequestHandler.GetAuthCredentials(IWebBrowser chromiumWebBrowser, IBrowser browser, string originUrl, bool isProxy, string host, int port, string realm, string scheme, IAuthCallback callback)
            {
                throw new NotImplementedException();
            }

            void IRequestHandler.OnRenderProcessTerminated(IWebBrowser chromiumWebBrowser, IBrowser browser, CefTerminationStatus status, int errorCode, string errorMessage)
            {
                throw new NotImplementedException();
            }
        }
    }
}
