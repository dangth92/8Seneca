using CefSharp;
using EightSeneca.WpfApp.ViewModels;
using EightSeneca.WpfApp.Views;
using System.Windows;

namespace EightSeneca.WpfApp
{
    public partial class App : Application
    {
        protected override async void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            var viewModel = new MainViewModel();
            var mainWindow = new MainWindow() { DataContext = viewModel };
            MainWindow = mainWindow;

            await viewModel.InitializeAsync(mainWindow);
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