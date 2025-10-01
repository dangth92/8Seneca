using System.Windows;
using EightSeneca.Wpf.Services;
using LibVLCSharp.Shared;

namespace EightSeneca.Wpf
{
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            Core.Initialize(); // for LibVLC

            // Init services
            var registry = new RegistryService();
            registry.CreateDefaults();
            var webEngine = new WebView2WebEngineService(registry);
            var media = new LibVlcMediaService();
            var tray = new TrayService();

            var vm = new ViewModels.MainViewModel(registry, tray, webEngine, media);
            var window = new Views.MainWindow { DataContext = vm };
            window.Hide();
        }
    }
}
