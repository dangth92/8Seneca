using System;

namespace EightSeneca.Wpf.Contracts
{
    public interface ITrayService : IDisposable
    {
        void Show();
        void Hide();
        void Close();
        event EventHandler ShowRequested;
        event EventHandler HideRequested;
        event EventHandler CloseRequested;
    }
}
