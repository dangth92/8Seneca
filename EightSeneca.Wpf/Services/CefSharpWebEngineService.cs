using CefSharp;
using CefSharp.Wpf;
using EightSeneca.Wpf.Contracts;
using System;

namespace EightSeneca.Wpf.Services
{
    public class CefSharpWebEngineService : IWebEngineService
    {
        public ChromiumWebBrowser Browser { get; private set; }
        public object BrowserControl => Browser;

        private readonly IRegistryService _registry;

        public CefSharpWebEngineService(IRegistryService registry)
        {
            _registry = registry;
            Browser = new ChromiumWebBrowser(_registry.GetString("Url") ?? "https://example.com");
        }

        public void Navigate(string url) => Browser.Load(url);
        public void EnableZoom(bool enable) { /* implement via settings */ }
        public void EnableTouch(bool enable) { /* implement via settings */ }
        public void AllowExternalLinks(bool allow) { /* implement via settings */ }
    }
}
