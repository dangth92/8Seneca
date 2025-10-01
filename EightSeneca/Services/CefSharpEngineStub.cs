using EightSeneca.Contracts;
using System.Windows;

namespace EightSeneca.Services;

public class CefSharpEngineStub : IWebEngine
{
    private readonly System.Windows.Controls.TextBlock txt;

    public CefSharpEngineStub()
    {
        txt = new System.Windows.Controls.TextBlock { Text = "CefSharp engine selected, but not implemented in this build.", TextWrapping = TextWrapping.Wrap };
    }

    public FrameworkElement GetView() => txt;
    public void Initialize() { }
    public void Navigate(string url) { txt.Text = $"CefSharp stub: would navigate to {url}"; }
    public void SetZoomEnabled(bool enabled) { }
    public void SetTouchEnabled(bool enabled) { }
    public void SetFollowExternalLinks(bool allowed) { }
}
