using CefSharp;
using CefSharp.Handler;
using System.Diagnostics;

namespace EightSeneca.WpfApp.Services
{
    public class CustomRequestHandler : RequestHandler
    {
        private readonly CefSharpService _service;

        public CustomRequestHandler(CefSharpService service)
        {
            _service = service;
        }

        protected override bool OnBeforeBrowse(
            IWebBrowser chromiumWebBrowser,
            IBrowser browser,
            IFrame frame,
            IRequest request,
            bool userGesture,
            bool isRedirect)
        {
            if (!_service.AllowExternalLinks)
            {
                var url = request.Url ?? string.Empty;
                if (_service.IsExternalNavigation(url))
                {
                    Debug.WriteLine($"[CefSharp] Blocked external navigation: {url}");
                    return true; // true = cancel navigation
                }
            }

            return false; // continue navigation
        }
    }
}
