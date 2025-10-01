using System;
using System.Windows;
using CefSharp;
using EightSeneca.WpfApp.Views;
using EightSeneca.WpfApp.ViewModels;
using CefSharp.Wpf;
using System.IO;
using System.Threading.Tasks;

namespace EightSeneca.WpfApp
{
    public partial class App : Application
    {
        private MainWindow _mainWindow;

        protected override async void OnStartup(StartupEventArgs e)
        {
            //InitializeCefSharp();

            base.OnStartup(e);

            _mainWindow = new MainWindow();
            MainWindow = _mainWindow;

            var viewModel = new MainViewModel();
            _mainWindow.DataContext = viewModel;

            await viewModel.InitializeAsync(_mainWindow);
        }

        private void InitializeCefSharp()
        {
            if (Cef.IsInitialized.GetValueOrDefault())
                return;

            var settings = new CefSettings
            {
                WindowlessRenderingEnabled = true,
                LogSeverity = LogSeverity.Disable,
                MultiThreadedMessageLoop = false,
            };

            // Thêm các setting cần thiết
            settings.CefCommandLineArgs.Add("disable-gpu", "1");
            //settings.CefCommandLineArgs.Add("disable-gpu-compositing", "1");

            try
            {
                Cef.Initialize(settings, performDependencyCheck: false, browserProcessHandler: null);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"CefSharp initialization failed: {ex.Message}");
                // Không throw, để app vẫn chạy với WebView2 fallback
            }
        }

        protected override void OnExit(ExitEventArgs e)
        {
            if (Cef.IsInitialized.GetValueOrDefault())
            {
                Cef.Shutdown();
            }
            base.OnExit(e);
        }
    }
}