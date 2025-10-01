using SenecaViewer.Services;
using SenecaViewer.ViewModels;
using System.Windows;

namespace SenecaViewer
{
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            // Initialize CefSharp
            var cefSettings = new CefSharp.Wpf.CefSettings();
            CefSharp.Cef.Initialize(cefSettings);

            // Create services
            var registryService = new RegistryService();
            var browserService = new BrowserService();
            var videoService = new VideoService();

            // Create ViewModel
            var viewModel = new MainViewModel(registryService, browserService, videoService);

            // Create and show main window with ViewModel
            var mainWindow = new MainWindow();
            mainWindow.DataContext = viewModel;
            mainWindow.Show();

            // Initialize after window is shown
            viewModel.Initialize();
        }

        protected override void OnExit(ExitEventArgs e)
        {
            CefSharp.Cef.Shutdown();
            base.OnExit(e);
        }
    }
}