using System.Windows;

namespace EightSeneca.Contracts;

public interface IWebEngine
{
    FrameworkElement GetView();
    void Initialize();
    void Navigate(string url);
    void SetZoomEnabled(bool enabled);
    void SetTouchEnabled(bool enabled);
    void SetFollowExternalLinks(bool allowed);
}
