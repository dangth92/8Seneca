using EightSeneca.Commands;
using EightSeneca.Wpf.Contracts;
using EightSeneca.Wpf.Services;
using LibVLCSharp.WPF;
using System.Windows.Input;

namespace EightSeneca.Wpf.ViewModels
{
    public class MainViewModel
    {
        public IWebEngineService WebEngine { get; }
        public LibVlcMediaService Media { get; }

        public ICommand ShowCommand { get; }
        public ICommand HideCommand { get; }
        public ICommand CloseCommand { get; }

        private ITrayService _tray;
        private IRegistryService _registry;

        public MainViewModel(IRegistryService registry, ITrayService tray, IWebEngineService webEngine, IMediaService mediaService)
        {
            _registry = registry;
            _tray = tray;
            WebEngine = webEngine;
            Media = mediaService as LibVlcMediaService;

            // Commands
            ShowCommand = new RelayCommand(_ => _tray.Show());
            HideCommand = new RelayCommand(_ => _tray.Hide());
            CloseCommand = new RelayCommand(_ => _tray.Close());

            // Tray events
            _tray.ShowRequested += (s, e) => ShowWindow();
            _tray.HideRequested += (s, e) => HideWindow();
            _tray.CloseRequested += (s, e) => CloseApp();
        }

        public VideoView Video1 { get; }
        public VideoView Video2 { get; }
        public VideoView Video3 { get; }

        private void ShowWindow()
        {
            var window = System.Windows.Application.Current.MainWindow;
            if (window != null)
            {
                window.Show();
                window.WindowState = System.Windows.WindowState.Maximized;
            }
        }

        private void HideWindow()
        {
            System.Windows.Application.Current.MainWindow?.Hide();
        }

        private void CloseApp()
        {
            System.Windows.Application.Current.Shutdown();
        }
    }
}
