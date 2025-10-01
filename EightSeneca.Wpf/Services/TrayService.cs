using EightSeneca.Wpf.Contracts;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace EightSeneca.Wpf.Services
{
    public class TrayService : ITrayService
    {
        private NotifyIcon _notifyIcon;

        public event EventHandler ShowRequested;
        public event EventHandler HideRequested;
        public event EventHandler CloseRequested;

        public TrayService()
        {
            _notifyIcon = new NotifyIcon();
            _notifyIcon.Icon = SystemIcons.Application; // Bạn có thể thay icon tùy chỉnh
            _notifyIcon.Visible = true;
            _notifyIcon.Text = "EightSeneca WPF";

            // Menu context
            var menu = new ContextMenuStrip();
            var showItem = new ToolStripMenuItem("Show/Hide");
            var closeItem = new ToolStripMenuItem("Close");

            showItem.Click += (s, e) => ShowRequested?.Invoke(this, EventArgs.Empty);
            closeItem.Click += (s, e) => CloseRequested?.Invoke(this, EventArgs.Empty);

            menu.Items.Add(showItem);
            menu.Items.Add(closeItem);

            _notifyIcon.ContextMenuStrip = menu;

            // Double click
            _notifyIcon.DoubleClick += (s, e) => ShowRequested?.Invoke(this, EventArgs.Empty);
        }

        public void Show() => ShowRequested?.Invoke(this, EventArgs.Empty);
        public void Hide() => HideRequested?.Invoke(this, EventArgs.Empty);
        public void Close()
        {
            CloseRequested?.Invoke(this, EventArgs.Empty);
            _notifyIcon.Visible = false;
            _notifyIcon.Dispose();
        }

        public void Dispose()
        {
            _notifyIcon?.Dispose();
        }
    }
}
