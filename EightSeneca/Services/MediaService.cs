using EightSeneca.Contracts;
using LibVLCSharp.Shared;
using LibVLCSharp.WPF;
using Microsoft.Win32;
using System;
using System.Windows;
using System.Windows.Controls;

namespace EightSeneca.Services;

public class MediaService : IMediaService
{
    private readonly LibVLC libVLC;
    private readonly MediaPlayer[] players = new MediaPlayer[3];
    private readonly VideoView[] views = new VideoView[3];
    private readonly IRegistryService registry;

    public MediaService(IRegistryService registry)
    {
        Core.Initialize(); // static initializer
        libVLC = new LibVLC();
        this.registry = registry;

        for (int i = 0; i < 3; i++)
        {
            players[i] = new MediaPlayer(libVLC);
            views[i] = new VideoView { MediaPlayer = players[i] };
        }
    }

    public FrameworkElement GetMediaView(int slot)
    {
        if (slot < 1 || slot > 3) throw new ArgumentOutOfRangeException(nameof(slot));
        return views[slot - 1];
    }

    public void PlayAllFromRegistry()
    {
        PlaySlot(1, registry.GetString("Video1Path"));
        PlaySlot(2, registry.GetString("Video2Path"));
        PlaySlot(3, registry.GetString("Video3Path"));
    }

    private void PlaySlot(int idx, string? path)
    {
        var player = players[idx - 1];
        if (!string.IsNullOrWhiteSpace(path))
        {
            try
            {
                var m = new Media(libVLC, new Uri(path));
                player.Stop();
                player.Play(m);
            }
            catch (Exception ex) { Console.WriteLine("LibVLC play error: " + ex.Message); }
        }
        else
        {
            player.Stop();
        }
    }

    public void Dispose()
    {
        foreach (var p in players) { p?.Stop(); p?.Dispose(); }
        libVLC?.Dispose();
    }
}
