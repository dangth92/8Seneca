using EightSeneca.RegistryTool;
using Microsoft.Win32;
using System;

namespace EightSeneca.RegistryTool
{
    public class RegistryService
    {
        private readonly RegistryKey baseKey;

        public RegistryService()
        {
            baseKey = Registry.CurrentUser.CreateSubKey(AppRegistryKeys.BasePath, true);
        }

        public void CreateDefaults()
        {
            baseKey.SetValue(AppRegistryKeys.WebUrl, AppRegistryKeys.DefaultWebUrl, RegistryValueKind.String);
            baseKey.SetValue(AppRegistryKeys.WebEngine, AppRegistryKeys.DefaultWebEngine, RegistryValueKind.String);
            baseKey.SetValue(AppRegistryKeys.EnableZoom, AppRegistryKeys.DefaultEnableZoom, RegistryValueKind.DWord);
            baseKey.SetValue(AppRegistryKeys.EnableTouch, AppRegistryKeys.DefaultEnableTouch, RegistryValueKind.DWord);
            baseKey.SetValue(AppRegistryKeys.FollowExternalLinks, AppRegistryKeys.DefaultFollowExternalLinks, RegistryValueKind.DWord);
            baseKey.SetValue(AppRegistryKeys.Video1Path, AppRegistryKeys.DefaultVideoPath, RegistryValueKind.String);
            baseKey.SetValue(AppRegistryKeys.Video2Path, AppRegistryKeys.DefaultVideoPath, RegistryValueKind.String);
            baseKey.SetValue(AppRegistryKeys.Video3Path, AppRegistryKeys.DefaultVideoPath, RegistryValueKind.String);
            Console.WriteLine("Default registry values created at HKCU\\" + AppRegistryKeys.BasePath);
        }

        public void List()
        {
            var names = baseKey.GetValueNames();
            if (names.Length == 0) { Console.WriteLine("No keys."); return; }
            foreach (var n in names)
            {
                var v = baseKey.GetValue(n);
                Console.WriteLine($"{n} = {v}");
            }
        }

        public void Change(string key, string value)
        {
            if (Array.IndexOf(baseKey.GetValueNames(), key) < 0)
            {
                Console.WriteLine($"Key '{key}' not found.");
                return;
            }
            // handle dword keys if necessary
            if (key == AppRegistryKeys.EnableZoom || key == AppRegistryKeys.EnableTouch || key == AppRegistryKeys.FollowExternalLinks)
            {
                if (int.TryParse(value, out int iv))
                    baseKey.SetValue(key, iv, RegistryValueKind.DWord);
                else
                {
                    Console.WriteLine("This key expects numeric 0/1.");
                    return;
                }
            }
            else
            {
                baseKey.SetValue(key, value, RegistryValueKind.String);
            }

            Console.WriteLine($"Set {key} = {value}");
        }

        public void Reset(string key)
        {
            switch (key)
            {
                case AppRegistryKeys.WebUrl:
                    baseKey.SetValue(AppRegistryKeys.WebUrl, AppRegistryKeys.DefaultWebUrl, RegistryValueKind.String);
                    break;
                case AppRegistryKeys.WebEngine:
                    baseKey.SetValue(AppRegistryKeys.WebEngine, AppRegistryKeys.DefaultWebEngine, RegistryValueKind.String);
                    break;
                case AppRegistryKeys.EnableZoom:
                    baseKey.SetValue(AppRegistryKeys.EnableZoom, AppRegistryKeys.DefaultEnableZoom, RegistryValueKind.DWord);
                    break;
                case AppRegistryKeys.EnableTouch:
                    baseKey.SetValue(AppRegistryKeys.EnableTouch, AppRegistryKeys.DefaultEnableTouch, RegistryValueKind.DWord);
                    break;
                case AppRegistryKeys.FollowExternalLinks:
                    baseKey.SetValue(AppRegistryKeys.FollowExternalLinks, AppRegistryKeys.DefaultFollowExternalLinks, RegistryValueKind.DWord);
                    break;
                case AppRegistryKeys.Video1Path:
                case AppRegistryKeys.Video2Path:
                case AppRegistryKeys.Video3Path:
                    baseKey.SetValue(key, AppRegistryKeys.DefaultVideoPath, RegistryValueKind.String);
                    break;
                default:
                    Console.WriteLine("Unknown key.");
                    return;
            }
            Console.WriteLine($"Reset {key} to default.");
        }

        public void RemoveAll()
        {
            try
            {
                Registry.CurrentUser.DeleteSubKeyTree(AppRegistryKeys.BasePath);
                Console.WriteLine("Removed registry key group.");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Remove failed: " + ex.Message);
            }
        }
    }
}
