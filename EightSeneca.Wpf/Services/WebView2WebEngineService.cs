using Microsoft.Web.WebView2.Wpf;
using EightSeneca.Wpf.Contracts;
using System;

namespace EightSeneca.Wpf.Services
{
    public class WebView2WebEngineService : IWebEngineService
    {
        public WebView2 Browser { get; private set; }
        public object BrowserControl => Browser;

        private readonly IRegistryService _registry;

        public WebView2WebEngineService(IRegistryService registry)
        {
            _registry = registry;
            Browser = new WebView2();
            Browser.Source = new Uri(_registry.GetString("Url") ?? "https://example.com");
        }

        public void Navigate(string url) => Browser.Source = new Uri(url);
        public void EnableZoom(bool enable) { /* implement */ }
        public void EnableTouch(bool enable) { /* implement */ }
        public void AllowExternalLinks(bool allow) { /* implement */ }
    }
}
