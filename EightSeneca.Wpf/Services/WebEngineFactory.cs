
namespace EightSeneca.Wpf.Services
{
    public class WebEngineFactory
    {
        private readonly RegistryService _registry;

        public WebEngineFactory(RegistryService registry) { _registry = registry; }

        public IWebEngine Create()
        {
            var eng = _registry.GetString("WebEngine") ?? "WebView2";
            if (eng.Equals("CefSharp", System.StringComparison.OrdinalIgnoreCase))
                return new CefSharpEngine();
            return new WebView2Engine();
        }
    }
}
