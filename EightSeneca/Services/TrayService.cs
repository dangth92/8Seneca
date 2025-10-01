using System.Windows;
using System.Windows.Forms;
using EightSeneca.Views;
using Application = System.Windows.Application;

namespace EightSeneca.Services;

public class TrayService
{
    private NotifyIcon? _notifyIcon;
    private MainWindow? _mainWindow;

    public void Initialize()
    {
        _mainWindow = new MainWindow();
        _mainWindow.Hide();

        _notifyIcon = new NotifyIcon
        {
            Icon = System.Drawing.SystemIcons.Application,
            Visible = true,
            Text = "EightSeneca"
        };

        var menu = new ContextMenuStrip();
        menu.Items.Add("Show/Hide", null, (s, e) => ToggleWindow());
        menu.Items.Add("Close", null, (s, e) => Application.Current.Shutdown());
        _notifyIcon.ContextMenuStrip = menu;
        _notifyIcon.DoubleClick += (s, e) => ShowWindow();
    }

    private void ToggleWindow()
    {
        if (_mainWindow == null) return;
        if (_mainWindow.IsVisible) _mainWindow.Hide();
        else ShowWindow();
    }

    private void ShowWindow()
    {
        if (_mainWindow == null) return;
        _mainWindow.Show();
        _mainWindow.WindowState = WindowState.Maximized;
        _mainWindow.Activate();
    }
}