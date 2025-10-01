using Microsoft.Win32;
using System;
using System.Collections.Generic;

namespace SenecaRegistry
{
    class Program
    {
        private const string REGISTRY_PATH = @"Software\8Seneca";

        static void Main(string[] args)
        {
            Console.WriteLine("=== 8Seneca Registry Manager ===\n");

            if (args.Length == 0)
            {
                ShowHelp();
                return;
            }

            string command = args[0].ToLower();

            try
            {
                switch (command)
                {
                    case "init":
                        CreateDefaultKeys();
                        break;
                    case "list":
                        ListKeys();
                        break;
                    case "set":
                        if (args.Length < 3)
                        {
                            Console.WriteLine("Usage: SenecaRegistry set <key> <value>");
                            return;
                        }
                        SetKey(args[1], args[2]);
                        break;
                    case "reset":
                        if (args.Length < 2)
                        {
                            Console.WriteLine("Usage: SenecaRegistry reset <key>");
                            return;
                        }
                        ResetKey(args[1]);
                        break;
                    case "wipe":
                        WipeKeys();
                        break;
                    default:
                        ShowHelp();
                        break;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
        }

        static void ShowHelp()
        {
            Console.WriteLine("Commands:");
            Console.WriteLine("  init              - Create registry keys with default values");
            Console.WriteLine("  list              - List all keys and values");
            Console.WriteLine("  set <key> <value> - Set key to new value");
            Console.WriteLine("  reset <key>       - Reset key to default value");
            Console.WriteLine("  wipe              - Remove all keys and group");
            Console.WriteLine("\nAvailable Keys:");
            Console.WriteLine("  BrowserUrl        - URL for web browser");
            Console.WriteLine("  WebEngine         - Web engine (WebView2 or CefSharp)");
            Console.WriteLine("  EnableZoom        - Enable/disable zoom (true/false)");
            Console.WriteLine("  EnableTouch       - Enable/disable touch (true/false)");
            Console.WriteLine("  AllowExternalLinks - Allow external links (true/false)");
            Console.WriteLine("  Video1Path        - Path to first video");
            Console.WriteLine("  Video2Path        - Path to second video");
            Console.WriteLine("  Video3Path        - Path to third video");
        }

        static void CreateDefaultKeys()
        {
            using (RegistryKey key = Registry.CurrentUser.CreateSubKey(REGISTRY_PATH))
            {
                if (key == null)
                {
                    Console.WriteLine("Failed to create registry key.");
                    return;
                }

                key.SetValue("BrowserUrl", "https://www.google.com", RegistryValueKind.String);
                key.SetValue("WebEngine", "WebView2", RegistryValueKind.String);
                key.SetValue("EnableZoom", "true", RegistryValueKind.String);
                key.SetValue("EnableTouch", "true", RegistryValueKind.String);
                key.SetValue("AllowExternalLinks", "false", RegistryValueKind.String);
                key.SetValue("Video1Path", "C:\\Videos\\video1.mp4", RegistryValueKind.String);
                key.SetValue("Video2Path", "C:\\Videos\\video2.mp4", RegistryValueKind.String);
                key.SetValue("Video3Path", "C:\\Videos\\video3.mp4", RegistryValueKind.String);

                Console.WriteLine("Registry keys created with default values at:");
                Console.WriteLine($"HKEY_CURRENT_USER\\{REGISTRY_PATH}");
            }
        }

        static void ListKeys()
        {
            using (RegistryKey key = Registry.CurrentUser.OpenSubKey(REGISTRY_PATH))
            {
                if (key == null)
                {
                    Console.WriteLine("Registry key does not exist. Run 'init' first.");
                    return;
                }

                Console.WriteLine($"Registry location: HKEY_CURRENT_USER\\{REGISTRY_PATH}\n");

                foreach (string valueName in key.GetValueNames())
                {
                    object value = key.GetValue(valueName);
                    Console.WriteLine($"{valueName,-20} = {value}");
                }
            }
        }

        static void SetKey(string keyName, string value)
        {
            using (RegistryKey key = Registry.CurrentUser.OpenSubKey(REGISTRY_PATH, true))
            {
                if (key == null)
                {
                    Console.WriteLine("Registry key does not exist. Run 'init' first.");
                    return;
                }

                key.SetValue(keyName, value, RegistryValueKind.String);
                Console.WriteLine($"Set {keyName} = {value}");
            }
        }

        static void ResetKey(string keyName)
        {
            var defaults = new Dictionary<string, string>
            {
                { "BrowserUrl", "https://www.google.com" },
                { "WebEngine", "WebView2" },
                { "EnableZoom", "true" },
                { "EnableTouch", "true" },
                { "AllowExternalLinks", "false" },
                { "Video1Path", "C:\\Videos\\video1.mp4" },
                { "Video2Path", "C:\\Videos\\video2.mp4" },
                { "Video3Path", "C:\\Videos\\video3.mp4" }
            };

            if (!defaults.ContainsKey(keyName))
            {
                Console.WriteLine($"Unknown key: {keyName}");
                return;
            }

            using (RegistryKey key = Registry.CurrentUser.OpenSubKey(REGISTRY_PATH, true))
            {
                if (key == null)
                {
                    Console.WriteLine("Registry key does not exist. Run 'init' first.");
                    return;
                }

                key.SetValue(keyName, defaults[keyName], RegistryValueKind.String);
                Console.WriteLine($"Reset {keyName} to default: {defaults[keyName]}");
            }
        }

        static void WipeKeys()
        {
            try
            {
                Registry.CurrentUser.DeleteSubKeyTree(REGISTRY_PATH);
                Console.WriteLine("All registry keys wiped successfully.");
            }
            catch (ArgumentException)
            {
                Console.WriteLine("Registry key does not exist.");
            }
        }
    }
}