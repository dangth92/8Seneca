using System;
using System.ComponentModel;
using System.Windows;
using System.Threading.Tasks;
using EightSeneca.WpfApp.Commands;
using EightSeneca.WpfApp.Services;

namespace EightSeneca.WpfApp.ViewModels
{
    public class MainViewModel : INotifyPropertyChanged
    {
        private readonly TrayService _trayService;
        private readonly VideoPlayerService _videoPlayerService;
        private readonly RegistryService _registryService;
        private readonly WebBrowserService _webBrowserService;
        private Window _mainWindow;

        private bool _isVideo1Loaded;
        private bool _isVideo2Loaded;
        private bool _isVideo3Loaded;

        public RelayCommand ShowWindowCommand { get; }
        public RelayCommand HideWindowCommand { get; }
        public RelayCommand CloseAppCommand { get; }

        public bool IsWindowVisible { get; private set; }
        public VideoPlayerService VideoPlayerService => _videoPlayerService;
        public WebBrowserService WebBrowserService => _webBrowserService;

        public bool IsVideo1Loaded
        {
            get => _isVideo1Loaded;
            private set
            {
                _isVideo1Loaded = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(ShowVideo1Placeholder));
            }
        }

        public bool IsVideo2Loaded
        {
            get => _isVideo2Loaded;
            private set
            {
                _isVideo2Loaded = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(ShowVideo2Placeholder));
            }
        }

        public bool IsVideo3Loaded
        {
            get => _isVideo3Loaded;
            private set
            {
                _isVideo3Loaded = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(ShowVideo3Placeholder));
            }
        }

        public bool ShowVideo1Placeholder => !IsVideo1Loaded;
        public bool ShowVideo2Placeholder => !IsVideo2Loaded;
        public bool ShowVideo3Placeholder => !IsVideo3Loaded;

        public event PropertyChangedEventHandler PropertyChanged;

        public MainViewModel()
        {
            ShowWindowCommand = new RelayCommand(_ => ShowWindow());
            HideWindowCommand = new RelayCommand(_ => HideWindow());
            CloseAppCommand = new RelayCommand(_ => CloseApp());

            _trayService = new TrayService(ShowWindow, CloseApp);
            _registryService = new RegistryService();
            _videoPlayerService = new VideoPlayerService();
            _webBrowserService = new WebBrowserService(_registryService);
        }

        public async Task InitializeAsync(Window mainWindow)
        {
            _mainWindow = mainWindow;
            ConfigureFullscreenWindow();

            await _webBrowserService.InitializeAsync();
            LoadVideosFromRegistry();
            HideWindow();
        }

        private void ConfigureFullscreenWindow()
        {
            _mainWindow.WindowStyle = WindowStyle.None;
            _mainWindow.ResizeMode = ResizeMode.NoResize;
            _mainWindow.WindowState = WindowState.Maximized;
        }

        private void LoadVideosFromRegistry()
        {
            string video1Path = _registryService.GetVideoPath(1);
            string video2Path = _registryService.GetVideoPath(2);
            string video3Path = _registryService.GetVideoPath(3);

            IsVideo1Loaded = System.IO.File.Exists(video1Path);
            IsVideo2Loaded = System.IO.File.Exists(video2Path);
            IsVideo3Loaded = System.IO.File.Exists(video3Path);

            if (IsVideo1Loaded) _videoPlayerService.PlayVideo(1, video1Path);
            if (IsVideo2Loaded) _videoPlayerService.PlayVideo(2, video2Path);
            if (IsVideo3Loaded) _videoPlayerService.PlayVideo(3, video3Path);
        }

        private void ShowWindow()
        {
            if (_mainWindow != null && !IsWindowVisible)
            {
                _mainWindow.WindowState = WindowState.Maximized;
                _mainWindow.Visibility = Visibility.Visible;
                _mainWindow.Activate();
                _videoPlayerService.ResumeAllVideos();
                IsWindowVisible = true;
            }
        }

        private void HideWindow()
        {
            if (_mainWindow != null && IsWindowVisible)
            {
                _videoPlayerService.PauseAllVideos();
                _mainWindow.Visibility = Visibility.Collapsed;
                _mainWindow.WindowState = WindowState.Minimized;
                IsWindowVisible = false;
            }
        }

        private void CloseApp()
        {
            _webBrowserService?.Dispose();
            _videoPlayerService?.Dispose();
            _trayService?.Dispose();
            Application.Current.Shutdown();
        }

        protected virtual void OnPropertyChanged(string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}