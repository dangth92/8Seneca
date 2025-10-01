using System.Windows;

namespace EightSeneca.Wpf.Services
{
    public interface IWebEngine
    {
        FrameworkElement GetView();
        void Initialize();
        void Navigate(string url);
        void SetZoomEnabled(bool enabled);
        void SetTouchEnabled(bool enabled);
        void SetFollowExternalLinks(bool allowed);
    }
}
