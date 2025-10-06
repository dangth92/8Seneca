using CefSharp;
using System.Windows.Input;

namespace EightSeneca.WpfApp.Services
{
    public class NoZoomKeyboardHandler : IKeyboardHandler
    {
        public bool OnKeyEvent(IWebBrowser chromiumWebBrowser, IBrowser browser, KeyType type, int windowsKeyCode, int nativeKeyCode, CefEventFlags modifiers, bool isSystemKey)
        {
            // Press Ctrl + '+', '-', or Wheel => ignore
            if ((modifiers & CefEventFlags.ControlDown) != 0)
            {
                if (windowsKeyCode == (int)Key.OemPlus || windowsKeyCode == (int)Key.Add ||
                    windowsKeyCode == (int)Key.OemMinus || windowsKeyCode == (int)Key.Subtract)
                    return true;
            }
            return false;
        }

        public bool OnPreKeyEvent(IWebBrowser chromiumWebBrowser, IBrowser browser, KeyType type, int windowsKeyCode, int nativeKeyCode, CefEventFlags modifiers, bool isSystemKey, ref bool isKeyboardShortcut)
            => false;
    }
}
