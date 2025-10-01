using System;
using System.Collections.Generic;
using Microsoft.Win32;
using EightSeneca.RegistryTool.Models;

namespace EightSeneca.RegistryTool.Services
{
    public class RegistryService
    {
        public void CreateRegistryKeys()
        {
            try
            {
                using (var baseKey = Registry.CurrentUser.CreateSubKey(RegistryConfig.REGISTRY_BASE_PATH))
                {
                    foreach (var keyDef in RegistryConfig.Keys)
                    {
                        if (baseKey.GetValue(keyDef.Name) == null)
                        {
                            baseKey.SetValue(keyDef.Name, keyDef.DefaultValue, keyDef.ValueKind);
                            Console.WriteLine($"Created key: {keyDef.Name} = {keyDef.DefaultValue}");
                        }
                        else
                        {
                            Console.WriteLine($"Key already exists: {keyDef.Name}");
                        }
                    }
                }
                Console.WriteLine("Registry keys created successfully.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error creating registry keys: {ex.Message}");
            }
        }

        public void ListRegistryKeys()
        {
            try
            {
                using (var baseKey = Registry.CurrentUser.OpenSubKey(RegistryConfig.REGISTRY_BASE_PATH))
                {
                    if (baseKey == null)
                    {
                        Console.WriteLine("No registry keys found. Please create them first using 'create' command.");
                        return;
                    }

                    Console.WriteLine("Registry Keys and Values:");
                    Console.WriteLine("=========================");

                    foreach (var keyDef in RegistryConfig.Keys)
                    {
                        var value = baseKey.GetValue(keyDef.Name, keyDef.DefaultValue);
                        Console.WriteLine($"{keyDef.Name}: {value} (Default: {keyDef.DefaultValue})");
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error listing registry keys: {ex.Message}");
            }
        }

        public void SetRegistryValue(string keyName, string newValue)
        {
            try
            {
                using (var baseKey = Registry.CurrentUser.OpenSubKey(RegistryConfig.REGISTRY_BASE_PATH, true))
                {
                    if (baseKey == null)
                    {
                        Console.WriteLine("Registry keys not found. Please create them first using 'create' command.");
                        return;
                    }

                    var keyDef = Array.Find(RegistryConfig.Keys, k => k.Name.Equals(keyName, StringComparison.OrdinalIgnoreCase));
                    if (keyDef == null)
                    {
                        Console.WriteLine($"Key '{keyName}' not found in registry configuration.");
                        return;
                    }

                    // Validate specific keys
                    string validationError = ValidateKeyValue(keyDef.Name, newValue);
                    if (!string.IsNullOrEmpty(validationError))
                    {
                        Console.WriteLine(validationError);
                        return;
                    }

                    baseKey.SetValue(keyDef.Name, newValue, keyDef.ValueKind);
                    Console.WriteLine($"Set {keyName} = {newValue}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error setting registry value: {ex.Message}");
            }
        }

        private string ValidateKeyValue(string keyName, string value)
        {
            switch (keyName.ToLower())
            {
                case "webengine":
                    if (!value.Equals("WebView2", StringComparison.OrdinalIgnoreCase) &&
                        !value.Equals("CefSharp", StringComparison.OrdinalIgnoreCase))
                    {
                        return "Error: WebEngine must be either 'WebView2' or 'CefSharp'.";
                    }
                    break;
                case "enablezoom":
                case "enabletouch":
                case "allowexternallinks":
                    if (!value.Equals("true", StringComparison.OrdinalIgnoreCase) &&
                        !value.Equals("false", StringComparison.OrdinalIgnoreCase))
                    {
                        return $"Error: {keyName} must be either 'true' or 'false'.";
                    }
                    break;
            }
            return null;
        }

        public void ResetRegistryValue(string keyName)
        {
            try
            {
                using (var baseKey = Registry.CurrentUser.OpenSubKey(RegistryConfig.REGISTRY_BASE_PATH, true))
                {
                    if (baseKey == null)
                    {
                        Console.WriteLine("Registry keys not found. Please create them first using 'create' command.");
                        return;
                    }

                    var keyDef = Array.Find(RegistryConfig.Keys, k => k.Name.Equals(keyName, StringComparison.OrdinalIgnoreCase));
                    if (keyDef == null)
                    {
                        Console.WriteLine($"Key '{keyName}' not found in registry configuration.");
                        return;
                    }

                    baseKey.SetValue(keyDef.Name, keyDef.DefaultValue, keyDef.ValueKind);
                    Console.WriteLine($"Reset {keyName} to default value: {keyDef.DefaultValue}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error resetting registry value: {ex.Message}");
            }
        }

        public void WipeRegistryKeys()
        {
            try
            {
                Registry.CurrentUser.DeleteSubKeyTree(RegistryConfig.REGISTRY_BASE_PATH);
                Console.WriteLine("Registry keys wiped successfully.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error wiping registry keys: {ex.Message}");
            }
        }
    }
}