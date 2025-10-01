using EightSeneca.Contracts;
using System.Windows.Controls;
using UserControl = System.Windows.Controls.UserControl;

namespace EightSeneca.Services
{
    public class WebEngineFactory : IWebEngineFactory
    {
        private readonly IRegistryService _registryService;

        public WebEngineFactory(IRegistryService registryService)
        {
            _registryService = registryService;
        }

        public UserControl Create()
        {
            var engine = _registryService.GetValue("WebEngine", "WebView2");
            if (engine == "CefSharp")
                return new CefSharpEngine(_registryService);
            return new WebView2Engine(_registryService);
        }
    }
}
