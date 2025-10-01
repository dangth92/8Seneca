using Microsoft.Win32;
using System;
using System.Threading;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using EightSeneca.Contracts;

namespace EightSeneca.Services;

public class RegistryService : IRegistryService
{
    private const string BasePath = @"Software\8SenecaWpfTask";
    private readonly RegistryKey baseKey;
    private readonly CancellationTokenSource cts = new();
    private readonly Dictionary<string, string?> previous = new();

    public event Action<string, string?>? KeyChanged;

    public RegistryService()
    {
        baseKey = Registry.CurrentUser.CreateSubKey(BasePath, true) ?? throw new Exception("Cannot open registry key.");
        CreateDefaultsIfMissing();

        // init previous snapshot
        foreach (var name in baseKey.GetValueNames())
            previous[name] = baseKey.GetValue(name)?.ToString();

        // start watcher (poll)
        Task.Run(() => WatchLoop(cts.Token));
    }

    public string? GetString(string key) => baseKey.GetValue(key) as string;
    public bool GetBool(string key)
    {
        var v = baseKey.GetValue(key);
        if (v is int i) return i != 0;
        if (v is string s && bool.TryParse(s, out var b)) return b;
        return false;
    }

    public void SetString(string key, string value) => baseKey.SetValue(key, value);

    public void CreateDefaultsIfMissing()
    {
        // set only if not exist
        EnsureValue("WebUrl", "https://example.com");
        EnsureValue("WebEngine", "WebView2");
        EnsureValue("EnableZoom", 1, RegistryValueKind.DWord);
        EnsureValue("EnableTouch", 1, RegistryValueKind.DWord);
        EnsureValue("FollowExternalLinks", 0, RegistryValueKind.DWord);
        EnsureValue("Video1Path", "");
        EnsureValue("Video2Path", "");
        EnsureValue("Video3Path", "");
    }

    private void EnsureValue(string name, object value, RegistryValueKind kind = RegistryValueKind.String)
    {
        if (baseKey.GetValue(name) == null)
            baseKey.SetValue(name, value, kind);
    }

    private void WatchLoop(CancellationToken token)
    {
        while (!token.IsCancellationRequested)
        {
            Thread.Sleep(2000);
            var names = baseKey.GetValueNames().ToList();
            foreach (var n in names)
            {
                var cur = baseKey.GetValue(n)?.ToString();
                previous.TryGetValue(n, out var prev);
                if (cur != prev)
                {
                    previous[n] = cur;
                    KeyChanged?.Invoke(n, cur);
                }
            }
        }
    }

    public void Dispose()
    {
        cts.Cancel();
        baseKey.Dispose();
    }
}
