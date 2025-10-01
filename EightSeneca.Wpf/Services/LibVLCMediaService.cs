using EightSeneca.Wpf.Contracts;
using LibVLCSharp.Shared;
using LibVLCSharp.WPF;
using System;

namespace EightSeneca.Wpf.Services
{
    public class LibVlcMediaService : IMediaService
    {
        private LibVLC _libVLC;
        private MediaPlayer[] _players;

        public MediaPlayer[] Players
        {
            get
            {
                return _players;
            }
        }

        public LibVlcMediaService()
        {
            Core.Initialize();
            _libVLC = new LibVLC();
            _players = new MediaPlayer[3];
            for (int i = 0; i < 3; i++)
                _players[i] = new MediaPlayer(_libVLC);
        }

        public void PlayVideo(int index, string path)
        {
            if (index < 0 || index >= 3) return;
            var media = new Media(_libVLC, path, FromType.FromPath);
            _players[index].Media = media;
            _players[index].Play();
        }

        public void StopVideo(int index)
        {
            if (index < 0 || index >= 3) return;
            _players[index].Stop();
        }
    }
}
