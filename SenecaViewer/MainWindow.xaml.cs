using SenecaViewer.Services;
using SenecaViewer.ViewModels;
using System.Windows;
using Hardcodet.Wpf.TaskbarNotification;

namespace SenecaViewer
{
    public partial class App : Application
    {
        private TaskbarIcon _trayIcon;
        private MainViewModel _viewModel;
        private MainWindow _mainWindow;

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
            _viewModel = new MainViewModel(registryService, browserService, videoService);

            // Create TRAY ICON FIRST (before window)
            _trayIcon = new TaskbarIcon();
            _trayIcon.IconSource = new System.Windows.Media.Imaging.BitmapImage(
                new System.Uri("pack://application:,,,/Assets/icon.ico"));
            _trayIcon.ToolTipText = "8Seneca Viewer";

            // Create context menu
            var contextMenu = new System.Windows.Controls.ContextMenu();

            var showHideMenuItem = new System.Windows.Controls.MenuItem();
            showHideMenuItem.Header = "Show/Hide";
            showHideMenuItem.Command = _viewModel.ToggleWindowCommand;

            var closeMenuItem = new System.Windows.Controls.MenuItem();
            closeMenuItem.Header = "Close";
            closeMenuItem.Command = _viewModel.CloseApplicationCommand;

            contextMenu.Items.Add(showHideMenuItem);
            contextMenu.Items.Add(closeMenuItem);

            _trayIcon.ContextMenu = contextMenu;

            // Double-click handler
            _trayIcon.TrayLeftMouseDoubleClick += (s, args) =>
            {
                _viewModel.ShowWindowCommand.Execute(null);
            };

            // Create main window (but don't show yet)
            _mainWindow = new MainWindow();
            _mainWindow.DataContext = _viewModel;

            // Subscribe to visibility changes
            _viewModel.PropertyChanged += (s, args) =>
            {
                if (args.PropertyName == nameof(MainViewModel.IsWindowVisible))
                {
                    if (_viewModel.IsWindowVisible)
                    {
                        ShowFullScreen();
                    }
                    else
                    {
                        HideWindow();
                    }
                }
            };

            // Initialize content (browser, videos)
            _viewModel.Initialize();

            // Keep app running even when window is hidden
            this.ShutdownMode = ShutdownMode.OnExplicitShutdown;
        }

        private void ShowFullScreen()
        {
            if (_mainWindow != null)
            {
                _mainWindow.WindowState = WindowState.Normal;
                _mainWindow.WindowStyle = WindowStyle.None;
                _mainWindow.ResizeMode = ResizeMode.NoResize;
                _mainWindow.Left = 0;
                _mainWindow.Top = 0;
                _mainWindow.Width = SystemParameters.PrimaryScreenWidth;
                _mainWindow.Height = SystemParameters.PrimaryScreenHeight;
                _mainWindow.Topmost = false; // KHÔNG topmost - cho phép Alt+Tab
                _mainWindow.Visibility = Visibility.Visible;
                _mainWindow.WindowState = WindowState.Maximized;
                _mainWindow.Activate();
                _mainWindow.Focus();
            }
        }

        private void HideWindow()
        {
            if (_mainWindow != null)
            {
                _mainWindow.Topmost = false;
                _mainWindow.Visibility = Visibility.Collapsed;
            }
        }

        protected override void OnExit(ExitEventArgs e)
        {
            _trayIcon?.Dispose();
            _viewModel?.Dispose();
            CefSharp.Cef.Shutdown();
            base.OnExit(e);
        }
    }
}