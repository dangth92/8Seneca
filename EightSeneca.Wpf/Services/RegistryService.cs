using Microsoft.Win32;
using EightSeneca.Wpf.Contracts;

namespace EightSeneca.Wpf.Services
{
    public class RegistryService : IRegistryService
    {
        private const string BaseRegistryPath = @"Software\EightSeneca";

        public string GetString(string keyName)
        {
            var key = Registry.CurrentUser.OpenSubKey(BaseRegistryPath);
            return key?.GetValue(keyName)?.ToString();
        }

        public int GetInt(string keyName)
        {
            var key = Registry.CurrentUser.OpenSubKey(BaseRegistryPath);
            if (key?.GetValue(keyName) is int val) return val;
            return 0;
        }

        public void SetString(string keyName, string value)
        {
            var key = Registry.CurrentUser.CreateSubKey(BaseRegistryPath);
            key.SetValue(keyName, value, RegistryValueKind.String);
        }

        public void SetInt(string keyName, int value)
        {
            var key = Registry.CurrentUser.CreateSubKey(BaseRegistryPath);
            key.SetValue(keyName, value, RegistryValueKind.DWord);
        }

        public void Reset(string keyName) => SetString(keyName, "");

        public void CreateDefaults()
        {
            var key = Registry.CurrentUser.CreateSubKey(BaseRegistryPath);
            key.SetValue("WebEngine", "WebView2", RegistryValueKind.String);
            key.SetValue("Url", "https://example.com", RegistryValueKind.String);
            key.SetValue("EnableZoom", 1, RegistryValueKind.DWord);
            key.SetValue("EnableTouch", 1, RegistryValueKind.DWord);
            key.SetValue("AllowExternalLinks", 0, RegistryValueKind.DWord);
        }

        public void RemoveAll()
        {
            Registry.CurrentUser.DeleteSubKeyTree(BaseRegistryPath, false);
        }
    }
}
