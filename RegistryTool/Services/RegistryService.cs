using Microsoft.Win32;
using RegistryTool.Models;

namespace RegistryTool.Services;
public class RegistryService
{
    private RegistryKey OpenBaseKey(bool writable = true)
    {
        return Registry.CurrentUser.CreateSubKey(AppRegistryKeys.BasePath, writable);
    }

    public void CreateDefaults()
    {
        using (var key = OpenBaseKey())
        {
            key.SetValue(AppRegistryKeys.UrlKey, AppRegistryKeys.DefaultUrl);
            key.SetValue(AppRegistryKeys.EngineKey, AppRegistryKeys.DefaultEngine);
            key.SetValue(AppRegistryKeys.ZoomEnabledKey, AppRegistryKeys.DefaultZoomEnabled, RegistryValueKind.DWord);
            key.SetValue(AppRegistryKeys.TouchEnabledKey, AppRegistryKeys.DefaultTouchEnabled, RegistryValueKind.DWord);
            key.SetValue(AppRegistryKeys.FollowExternalLinkKey, AppRegistryKeys.DefaultFollowExternalLink, RegistryValueKind.DWord);
        }
        Console.WriteLine("Default registry keys created.");
    }

    public void List()
    {
        using (var key = Registry.CurrentUser.OpenSubKey(AppRegistryKeys.BasePath))
        {
            if (key == null)
            {
                Console.WriteLine("No keys found.");
                return;
            }

            foreach (var name in key.GetValueNames())
            {
                var value = key.GetValue(name);
                Console.WriteLine($"{name} = {value}");
            }
        }
    }

    public void Change(string keyName, string value)
    {
        using (var key = OpenBaseKey())
        {
            if (key.GetValue(keyName) == null)
            {
                Console.WriteLine($"Key '{keyName}' does not exist.");
                return;
            }
            key.SetValue(keyName, value);
            Console.WriteLine($"Key '{keyName}' updated to {value}");
        }
    }

    public void Reset(string keyName)
    {
        using (var key = OpenBaseKey())
        {
            switch (keyName)
            {
                case nameof(AppRegistryKeys.UrlKey):
                    key.SetValue(AppRegistryKeys.UrlKey, AppRegistryKeys.DefaultUrl);
                    break;
                case nameof(AppRegistryKeys.EngineKey):
                    key.SetValue(AppRegistryKeys.EngineKey, AppRegistryKeys.DefaultEngine);
                    break;
                case nameof(AppRegistryKeys.ZoomEnabledKey):
                    key.SetValue(AppRegistryKeys.ZoomEnabledKey, AppRegistryKeys.DefaultZoomEnabled, RegistryValueKind.DWord);
                    break;
                case nameof(AppRegistryKeys.TouchEnabledKey):
                    key.SetValue(AppRegistryKeys.TouchEnabledKey, AppRegistryKeys.DefaultTouchEnabled, RegistryValueKind.DWord);
                    break;
                case nameof(AppRegistryKeys.FollowExternalLinkKey):
                    key.SetValue(AppRegistryKeys.FollowExternalLinkKey, AppRegistryKeys.DefaultFollowExternalLink, RegistryValueKind.DWord);
                    break;
                default:
                    Console.WriteLine($"Unknown key {keyName}");
                    return;
            }
            Console.WriteLine($"Key '{keyName}' reset to default value.");
        }
    }

    public void RemoveAll()
    {
        Registry.CurrentUser.DeleteSubKeyTree(AppRegistryKeys.BasePath, false);
        Console.WriteLine("All keys removed.");
    }
}
