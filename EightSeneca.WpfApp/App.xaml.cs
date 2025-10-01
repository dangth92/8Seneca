using System;
using System.Windows;
using CefSharp;
using EightSeneca.WpfApp.Views;
using EightSeneca.WpfApp.ViewModels;
using CefSharp.Wpf;

namespace EightSeneca.WpfApp
{
    public partial class App : Application
    {
        private MainWindow _mainWindow;

        protected override async void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            InitializeCefSharp();

            _mainWindow = new MainWindow();
            MainWindow = _mainWindow;

            var viewModel = new MainViewModel();
            _mainWindow.DataContext = viewModel;

            await viewModel.InitializeAsync(_mainWindow);
        }

        private void InitializeCefSharp()
        {
            //if (Cef.IsInitialized.GetValueOrDefault()) return;

            //try
            //{
            //    Cef.Initialize(new CefSettings());
            //}
            //catch
            //{
            //}
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