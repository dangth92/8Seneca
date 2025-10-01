using System;
using System.Windows;
using Hardcodet.Wpf.TaskbarNotification;
using EightSeneca.WpfApp.Commands;

namespace EightSeneca.WpfApp.Services
{
    public class TrayService : IDisposable
    {
        private readonly TaskbarIcon _taskbarIcon;
        private readonly Action _showWindowAction;
        private readonly Action _closeAppAction;

        public TrayService(Action showWindowAction, Action closeAppAction)
        {
            _showWindowAction = showWindowAction;
            _closeAppAction = closeAppAction;

            _taskbarIcon = new TaskbarIcon();
            InitializeTrayIcon();
        }

        private void InitializeTrayIcon()
        {
            _taskbarIcon.ToolTipText = "EightSeneca App";

            SetTrayIcon();

            var contextMenu = new System.Windows.Controls.ContextMenu();

            var showHideMenuItem = new System.Windows.Controls.MenuItem
            {
                Header = "Show/Hide",
                Command = new RelayCommand(_ => _showWindowAction())
            };

            var closeMenuItem = new System.Windows.Controls.MenuItem
            {
                Header = "Close",
                Command = new RelayCommand(_ => _closeAppAction())
            };

            contextMenu.Items.Add(showHideMenuItem);
            contextMenu.Items.Add(closeMenuItem);

            _taskbarIcon.ContextMenu = contextMenu;
            _taskbarIcon.DoubleClickCommand = new RelayCommand(_ => _showWindowAction());
        }

        private void SetTrayIcon()
        {
            try
            {
                var iconUri = new Uri("pack://application:,,,/Assets/app.ico", UriKind.Absolute);
                var iconStream = Application.GetResourceStream(iconUri);
                if (iconStream != null)
                {
                    using (var stream = iconStream.Stream)
                    {
                        _taskbarIcon.Icon = new System.Drawing.Icon(stream);
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Could not load icon: {ex.Message}");

                // CÁCH 2: Tạo icon programmatically như fallback
                CreateDefaultIcon();
            }
        }

        private void CreateDefaultIcon()
        {
            try
            {
                using (var bitmap = new System.Drawing.Bitmap(16, 16))
                using (var graphics = System.Drawing.Graphics.FromImage(bitmap))
                {
                    graphics.Clear(System.Drawing.Color.DodgerBlue);

                    using (var font = new System.Drawing.Font("Arial", 10, System.Drawing.FontStyle.Bold))
                    using (var brush = new System.Drawing.SolidBrush(System.Drawing.Color.White))
                    {
                        graphics.DrawString("E", font, brush, 3, 1);
                    }
                    System.IntPtr hIcon = bitmap.GetHicon();
                    _taskbarIcon.Icon = System.Drawing.Icon.FromHandle(hIcon);
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Could not create default icon: {ex.Message}");
            }
        }

        public void Dispose()
        {
            _taskbarIcon?.Dispose();
        }
    }
}