using LibVLCSharp.Shared;
using System;
using System.Collections.Generic;
using System.IO;

namespace SenecaViewer.Services
{
    public class VideoService : IDisposable
    {
        private LibVLC _libVLC;
        private readonly List<MediaPlayer> _mediaPlayers = new List<MediaPlayer>();

        public LibVLC GetLibVLC()
        {
            if (_libVLC == null)
            {
                Core.Initialize();
                _libVLC = new LibVLC();
            }
            return _libVLC;
        }

        public MediaPlayer CreateMediaPlayer(string videoPath)
        {
            if (!File.Exists(videoPath))
            {
                throw new FileNotFoundException($"Video file not found: {videoPath}");
            }

            var libVLC = GetLibVLC();
            var media = new Media(libVLC, videoPath, FromType.FromPath);
            var mediaPlayer = new MediaPlayer(media);

            // Disable user input on video
            mediaPlayer.EnableMouseInput = false;
            mediaPlayer.EnableKeyInput = false;

            // Setup looping
            mediaPlayer.EndReached += (s, e) =>
            {
                mediaPlayer.Stop();
                mediaPlayer.Play(media);
            };

            _mediaPlayers.Add(mediaPlayer);
            return mediaPlayer;
        }

        public void Cleanup()
        {
            foreach (var player in _mediaPlayers)
            {
                try
                {
                    player.Stop();
                    player.Dispose();
                }
                catch { }
            }
            _mediaPlayers.Clear();

            if (_libVLC != null)
            {
                _libVLC.Dispose();
                _libVLC = null;
            }
        }

        public void Dispose()
        {
            Cleanup();
        }
    }
}