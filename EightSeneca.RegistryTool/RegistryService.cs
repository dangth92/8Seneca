using System;
using Microsoft.Win32;

namespace EightSeneca.RegistryTool
{
    public class RegistryService
    {
        private const string REGISTRY_PATH = @"Software\EightSeneca";

        private readonly string[] _registryKeys = {
            "BrowserUrl", "WebEngine", "EnableZoom",
            "EnableTouch", "AllowExternalLinks",
            "Video1Path", "Video2Path", "Video3Path"
        };

        private readonly string[] _defaultValues = {
            "https://www.google.com", "WebView2", "true",
            "true", "false",
            @"C:\Videos\video1.mp4", @"C:\Videos\video2.mp4", @"C:\Videos\video3.mp4"
        };

        public void CreateKeys()
        {
            try
            {
                using (var key = Registry.CurrentUser.CreateSubKey(REGISTRY_PATH))
                {
                    for (int i = 0; i < _registryKeys.Length; i++)
                    {
                        if (key.GetValue(_registryKeys[i]) == null)
                        {
                            key.SetValue(_registryKeys[i], _defaultValues[i]);
                            Console.WriteLine($"Created: {_registryKeys[i]} = {_defaultValues[i]}");
                        }
                    }
                }
                Console.WriteLine("Registry keys created successfully.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
        }

        public void ListKeys()
        {
            try
            {
                using (var key = Registry.CurrentUser.OpenSubKey(REGISTRY_PATH))
                {
                    if (key == null)
                    {
                        Console.WriteLine("No registry keys found. Use 'create' first.");
                        return;
                    }

                    Console.WriteLine("Registry Keys:");
                    for (int i = 0; i < _registryKeys.Length; i++)
                    {
                        var value = key.GetValue(_registryKeys[i], _defaultValues[i]);
                        Console.WriteLine($"{_registryKeys[i]}: {value}");
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
        }

        public void SetValue(string keyName, string newValue)
        {
            try
            {
                using (var key = Registry.CurrentUser.OpenSubKey(REGISTRY_PATH, true))
                {
                    if (key == null)
                    {
                        Console.WriteLine("Registry keys not found. Use 'create' first.");
                        return;
                    }

                    if (Array.IndexOf(_registryKeys, keyName) == -1)
                    {
                        Console.WriteLine($"Key '{keyName}' not found.");
                        return;
                    }

                    // Validate values
                    if (keyName == "WebEngine" && newValue != "WebView2" && newValue != "CefSharp")
                    {
                        Console.WriteLine("WebEngine must be 'WebView2' or 'CefSharp'");
                        return;
                    }

                    if ((keyName == "EnableZoom" || keyName == "EnableTouch" || keyName == "AllowExternalLinks")
                        && newValue != "true" && newValue != "false")
                    {
                        Console.WriteLine($"{keyName} must be 'true' or 'false'");
                        return;
                    }

                    key.SetValue(keyName, newValue);
                    Console.WriteLine($"Set {keyName} = {newValue}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
        }

        public void ResetValue(string keyName)
        {
            try
            {
                using (var key = Registry.CurrentUser.OpenSubKey(REGISTRY_PATH, true))
                {
                    if (key == null)
                    {
                        Console.WriteLine("Registry keys not found. Use 'create' first.");
                        return;
                    }

                    int index = Array.IndexOf(_registryKeys, keyName);
                    if (index == -1)
                    {
                        Console.WriteLine($"Key '{keyName}' not found.");
                        return;
                    }

                    key.SetValue(keyName, _defaultValues[index]);
                    Console.WriteLine($"Reset {keyName} = {_defaultValues[index]}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
        }

        public void WipeKeys()
        {
            try
            {
                Registry.CurrentUser.DeleteSubKeyTree(REGISTRY_PATH);
                Console.WriteLine("All registry keys removed.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
        }
    }
}