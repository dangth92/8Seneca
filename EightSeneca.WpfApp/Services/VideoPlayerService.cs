using LibVLCSharp.Shared;
using System;

namespace EightSeneca.WpfApp.Services
{
    /// <summary>
    /// Service for managing video playback using LibVLC engine
    /// Handles multiple video instances with H.264/H.265 support
    /// </summary>
    public class VideoPlayerService : IDisposable
    {
        private LibVLC _libVLC;
        private MediaPlayer _video1Player;
        private MediaPlayer _video2Player;
        private MediaPlayer _video3Player;

        // Public properties for binding to VideoView controls
        public MediaPlayer Video1Player => _video1Player;
        public MediaPlayer Video2Player => _video2Player;
        public MediaPlayer Video3Player => _video3Player;

        public VideoPlayerService()
        {
            InitializeLibVLC();
        }

        /// <summary>
        /// Initialize LibVLC engine with optimal settings for multiple videos
        /// </summary>
        private void InitializeLibVLC()
        {
            try
            {
                Core.Initialize();

                // Configure LibVLC for multiple video playback
                _libVLC = new LibVLC("--no-xlib", "--avcodec-hw=any", "--drop-late-frames", "--skip-frames");

                // Create media players for each video
                _video1Player = new MediaPlayer(_libVLC);
                _video2Player = new MediaPlayer(_libVLC);
                _video3Player = new MediaPlayer(_libVLC);

                System.Diagnostics.Debug.WriteLine("LibVLC initialized successfully");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Failed to initialize LibVLC: {ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// Get MediaPlayer instance for specified video index (1, 2, or 3)
        /// </summary>
        public MediaPlayer GetVideoPlayer(int videoIndex)
        {
            switch (videoIndex)
            {
                case 1:
                    return _video1Player;
                case 2:
                    return _video2Player;
                case 3:
                    return _video3Player;
                default:
                    throw new ArgumentException("Video index must be 1, 2, or 3");
            }
        }

        /// <summary>
        /// Load and play video file for specified video index
        /// </summary>
        public void PlayVideo(int videoIndex, string videoPath)
        {
            try
            {
                var player = GetVideoPlayer(videoIndex);

                if (System.IO.File.Exists(videoPath))
                {
                    using (var media = new Media(_libVLC, new Uri(videoPath)))
                    {
                        player.Play(media);
                        System.Diagnostics.Debug.WriteLine($"Playing video {videoIndex}: {videoPath}");
                    }
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine($"Video file not found: {videoPath}");
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error playing video {videoIndex}: {ex.Message}");
            }
        }

        /// <summary>
        /// Stop all video playback
        /// </summary>
        public void StopAllVideos()
        {
            _video1Player?.Stop();
            _video2Player?.Stop();
            _video3Player?.Stop();
        }

        /// <summary>
        /// Pause all video playback
        /// </summary>
        public void PauseAllVideos()
        {
            _video1Player?.Pause();
            _video2Player?.Pause();
            _video3Player?.Pause();
        }

        /// <summary>
        /// Resume all video playback
        /// </summary>
        public void ResumeAllVideos()
        {
            _video1Player?.Play();
            _video2Player?.Play();
            _video3Player?.Play();
        }

        public void Dispose()
        {
            _video1Player?.Dispose();
            _video2Player?.Dispose();
            _video3Player?.Dispose();
            _libVLC?.Dispose();
        }
    }
}