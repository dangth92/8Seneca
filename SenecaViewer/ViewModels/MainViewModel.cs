using LibVLCSharp.Shared;
using SenecaViewer.Helpers;
using SenecaViewer.Services;
using System;
using System.Windows;
using System.Windows.Input;

namespace SenecaViewer.ViewModels
{
    public class MainViewModel : ViewModelBase, IDisposable
    {
        private readonly RegistryService _registryService;
        private readonly BrowserService _browserService;
        private readonly VideoService _videoService;

        // Properties with backing fields
        private bool _isWindowVisible;
        public bool IsWindowVisible
        {
            get { return _isWindowVisible; }
            set { SetProperty(ref _isWindowVisible, value); }
        }

        private UIElement _browserControl;
        public UIElement BrowserControl
        {
            get { return _browserControl; }
            set { SetProperty(ref _browserControl, value); }
        }

        private MediaPlayer _videoPlayer1;
        public MediaPlayer VideoPlayer1
        {
            get { return _videoPlayer1; }
            set { SetProperty(ref _videoPlayer1, value); }
        }

        private MediaPlayer _videoPlayer2;
        public MediaPlayer VideoPlayer2
        {
            get { return _videoPlayer2; }
            set { SetProperty(ref _videoPlayer2, value); }
        }

        private MediaPlayer _videoPlayer3;
        public MediaPlayer VideoPlayer3
        {
            get { return _videoPlayer3; }
            set { SetProperty(ref _videoPlayer3, value); }
        }

        // Commands
        public ICommand ShowWindowCommand { get; }
        public ICommand HideWindowCommand { get; }
        public ICommand ToggleWindowCommand { get; }
        public ICommand CloseApplicationCommand { get; }

        public MainViewModel(RegistryService registryService, BrowserService browserService, VideoService videoService)
        {
            _registryService = registryService;
            _browserService = browserService;
            _videoService = videoService;

            // Initialize commands
            ShowWindowCommand = new RelayCommand(ShowWindow);
            HideWindowCommand = new RelayCommand(HideWindow);
            ToggleWindowCommand = new RelayCommand(ToggleWindow);
            CloseApplicationCommand = new RelayCommand(CloseApplication);

            // Start hidden
            IsWindowVisible = false;

            // Subscribe to URL changes
            _registryService.UrlChanged += OnUrlChanged;
            _registryService.StartMonitoring();
        }

        public void Initialize()
        {
            InitializeBrowser();
            InitializeVideos();
        }

        private void InitializeBrowser()
        {
            try
            {
                string url = _registryService.GetBrowserUrl();
                string engine = _registryService.GetWebEngine();
                bool enableZoom = _registryService.GetEnableZoom();
                bool allowExternalLinks = _registryService.GetAllowExternalLinks();

                BrowserControl = _browserService.CreateBrowser(engine, url, enableZoom, allowExternalLinks);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error initializing browser: {ex.Message}", "Error",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void InitializeVideos()
        {
            try
            {
                string video1Path = _registryService.GetVideoPath(1);
                string video2Path = _registryService.GetVideoPath(2);
                string video3Path = _registryService.GetVideoPath(3);

                VideoPlayer1 = CreateVideoPlayer(video1Path);
                VideoPlayer2 = CreateVideoPlayer(video2Path);
                VideoPlayer3 = CreateVideoPlayer(video3Path);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error initializing videos: {ex.Message}", "Error",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private MediaPlayer CreateVideoPlayer(string videoPath)
        {
            try
            {
                var player = _videoService.CreateMediaPlayer(videoPath);
                player.Play();
                return player;
            }
            catch
            {
                return null;
            }
        }

        private void OnUrlChanged(object sender, string newUrl)
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                if (BrowserControl != null)
                {
                    _browserService.NavigateToUrl(BrowserControl, newUrl);
                }
            });
        }

        private void ShowWindow()
        {
            IsWindowVisible = true;
        }

        private void HideWindow()
        {
            IsWindowVisible = false;
        }

        private void ToggleWindow()
        {
            IsWindowVisible = !IsWindowVisible;
        }

        private void CloseApplication()
        {
            Dispose();
            Application.Current.Shutdown();
        }

        public void Dispose()
        {
            _registryService.StopMonitoring();

            if (VideoPlayer1 != null) VideoPlayer1.Stop();
            if (VideoPlayer2 != null) VideoPlayer2.Stop();
            if (VideoPlayer3 != null) VideoPlayer3.Stop();

            _videoService.Cleanup();
            _registryService.Dispose();
            _videoService.Dispose();
        }
    }
}