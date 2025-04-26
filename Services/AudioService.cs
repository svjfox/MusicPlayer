using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Plugin.Maui.Audio;
using MusicPlayer.Models;
using MusicPlayer.Extensions;
using Microsoft.Maui.ApplicationModel;
using Microsoft.Maui.Storage;
using System.Linq;

namespace MusicPlayer.Services
{
    public class AudioService : IAudioService
    {
        private readonly IAudioManager _audioManager;
        private IAudioPlayer _audioPlayer;
        private System.Timers.Timer _positionTimer;
        private List<Track> _playlist = new();
        private int _currentTrackIndex = -1;
        private bool _isDisposed;
        private bool _isShuffleEnabled;
        private bool _isRepeatEnabled;
        private CancellationTokenSource _sleepTimerCts;

        public bool IsPlaying { get; private set; }
        public double CurrentPosition => _audioPlayer?.CurrentPosition ?? 0;
        public double Duration => _audioPlayer?.Duration ?? 0;
        public Track CurrentTrack { get; private set; }
        public IReadOnlyList<Track> CurrentPlaylist => _playlist.AsReadOnly();
        public bool IsShuffleEnabled => _isShuffleEnabled;
        public bool IsRepeatEnabled => _isRepeatEnabled;
        public bool IsSleepTimerActive => _sleepTimerCts != null && !_sleepTimerCts.IsCancellationRequested;

        public event EventHandler<bool> IsPlayingChanged;
        public event EventHandler<double> PositionChanged;
        public event EventHandler<Track> TrackChanged;
        public event EventHandler PlaybackEnded;
        public event EventHandler<bool> ShuffleChanged;
        public event EventHandler<bool> RepeatChanged;
        public event EventHandler SleepTimerActivated;

        public AudioService(IAudioManager audioManager)
        {
            _audioManager = audioManager;
            _positionTimer = new System.Timers.Timer(500);
            _positionTimer.Elapsed += OnPositionTimerElapsed;
        }

        public async Task TogglePlay()
        {
            if (IsPlaying)
            {
                await PauseAsync();
            }
            else
            {
                await PlayAsync();
            }
        }

        public async Task InitializeAsync(Stream initialStream = null)
        {
            if (_audioPlayer != null) return;

            try
            {
                // Create silent WAV stream for initialization
                var silentWav = new byte[] {
                                        0x52, 0x49, 0x46, 0x46, 0x24, 0x00, 0x00, 0x00, 0x57, 0x41, 0x56, 0x45,
                                        0x66, 0x6D, 0x74, 0x20, 0x10, 0x00, 0x00, 0x00, 0x01, 0x00, 0x01, 0x00,
                                        0x44, 0xAC, 0x00, 0x00, 0x88, 0x58, 0x01, 0x00, 0x02, 0x00, 0x10, 0x00,
                                        0x64, 0x61, 0x74, 0x61, 0x00, 0x00, 0x00, 0x00
                                    };

                using var ms = new MemoryStream(silentWav);
                _audioPlayer = _audioManager.CreatePlayer(ms);
                _audioPlayer.PlaybackEnded += OnPlaybackEnded;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Audio initialization error: {ex.Message}");
                throw;
            }
        }

        public async Task SetPlaylistAsync(IEnumerable<Track> tracks, int startIndex = 0)
        {
            _playlist = new List<Track>(tracks);
            _currentTrackIndex = Math.Clamp(startIndex, 0, _playlist.Count - 1);

            if (_playlist.Any())
            {
                await LoadTrackAsync(_playlist[_currentTrackIndex]);
            }
        }

        public async Task LoadTrackAsync(Track track)
        {
            if (track == null) return;

            try
            {
                // Stop current playback
                if (_audioPlayer != null)
                {
                    _audioPlayer.Pause();
                    _audioPlayer.Dispose();
                }

                // Load new track
                var stream = await FileSystem.OpenAppPackageFileAsync(track.Path);
                _audioPlayer = _audioManager.CreatePlayer(stream);
                _audioPlayer.PlaybackEnded += OnPlaybackEnded;

                // Update current track
                CurrentTrack = track;
                TrackChanged?.Invoke(this, track);

                // Update notification (Android)
                UpdateNotification();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading track: {ex.Message}");
                throw;
            }
        }

        public async Task PlayAsync()
        {
            if (_audioPlayer == null || CurrentTrack == null) return;

            _audioPlayer.Play();
            IsPlaying = true;
            _positionTimer.Start();
            IsPlayingChanged?.Invoke(this, true);
            UpdateNotification();
        }

        public async Task PauseAsync()
        {
            if (_audioPlayer == null) return;

            _audioPlayer.Pause();
            IsPlaying = false;
            _positionTimer.Stop();
            IsPlayingChanged?.Invoke(this, false);
            UpdateNotification();
        }

        public async Task StopAsync()
        {
            if (_audioPlayer == null) return;

            _audioPlayer.Stop();
            IsPlaying = false;
            _positionTimer.Stop();
            IsPlayingChanged?.Invoke(this, false);
            UpdateNotification();
        }

        public async Task NextAsync()
        {
            if (!_playlist.Any()) return;

            if (_isShuffleEnabled)
            {
                _currentTrackIndex = new Random().Next(0, _playlist.Count);
            }
            else
            {
                _currentTrackIndex = (_currentTrackIndex + 1) % _playlist.Count;
            }

            await LoadTrackAsync(_playlist[_currentTrackIndex]);
            await PlayAsync();
        }

        public async Task PreviousAsync()
        {
            if (!_playlist.Any()) return;

            // If track played less than 3 seconds, restart it
            if (CurrentPosition > 3)
            {
                await SeekToAsync(0);
                return;
            }

            if (_isShuffleEnabled)
            {
                _currentTrackIndex = new Random().Next(0, _playlist.Count);
            }
            else
            {
                _currentTrackIndex = (_currentTrackIndex - 1 + _playlist.Count) % _playlist.Count;
            }

            await LoadTrackAsync(_playlist[_currentTrackIndex]);
            await PlayAsync();
        }

        public async Task SeekToAsync(double position)
        {
            if (_audioPlayer == null) return;

            MainThread.BeginInvokeOnMainThread(() =>
            {
                _audioPlayer.Seek(position);
            });

            PositionChanged?.Invoke(this, position);
        }

        public async Task SetVolumeAsync(double volume)
        {
            if (_audioPlayer == null) return;

            volume = Math.Clamp(volume, 0, 1);
            _audioPlayer.Volume = volume;
            await SecureStorage.SetAsync("player_volume", volume.ToString());
        }

        public async Task SetSpeedAsync(double speed)
        {
            if (_audioPlayer == null) return;

            speed = Math.Clamp(speed, 0.5, 2.0);
            _audioPlayer.Speed = speed;
            await SecureStorage.SetAsync("player_speed", speed.ToString());
        }

        public async Task ToggleShuffleAsync()
        {
            _isShuffleEnabled = !_isShuffleEnabled;
            ShuffleChanged?.Invoke(this, _isShuffleEnabled);
            await SecureStorage.SetAsync("player_shuffle", _isShuffleEnabled.ToString());
        }

        public async Task ToggleRepeatAsync()
        {
            _isRepeatEnabled = !_isRepeatEnabled;
            RepeatChanged?.Invoke(this, _isRepeatEnabled);
            await SecureStorage.SetAsync("player_repeat", _isRepeatEnabled.ToString());
        }

        public async Task SetSleepTimerAsync(int minutes)
        {
            _sleepTimerCts?.Cancel();

            if (minutes <= 0) return;

            _sleepTimerCts = new CancellationTokenSource();
            try
            {
                await Task.Delay(TimeSpan.FromMinutes(minutes), _sleepTimerCts.Token);
                await PauseAsync();
                SleepTimerActivated?.Invoke(this, true);
            }
            catch (TaskCanceledException)
            {
                // Timer was canceled
            }
        }

        public async Task CancelSleepTimer()
        {
            _sleepTimerCts?.Cancel();
        }

        private void UpdateNotification()
        {
#if ANDROID
            if (CurrentTrack == null || DeviceInfo.Platform != DevicePlatform.Android)
                return;

            _audioPlayer?.SetNotificationOptions(
                title: CurrentTrack.Title,
                subtitle: CurrentTrack.Artist,
                description: CurrentTrack.Album,
                image: CurrentTrack.Artwork,
                actions: new List<NotificationAction>
                {
                                        new(1, "Previous", Android.Resource.Drawable.IcMediaPrevious),
                                        new(2, IsPlaying ? "Pause" : "Play",
                                            IsPlaying ? Android.Resource.Drawable.IcMediaPause : Android.Resource.Drawable.IcMediaPlay),
                                        new(3, "Next", Android.Resource.Drawable.IcMediaNext)
                });
#endif
        }

        private void OnPositionTimerElapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            PositionChanged?.Invoke(this, CurrentPosition);
        }

        private async void OnPlaybackEnded(object sender, EventArgs e)
        {
            if (_isRepeatEnabled && CurrentTrack != null)
            {
                await SeekToAsync(0);
                await PlayAsync();
                return;
            }

            PlaybackEnded?.Invoke(this, e);
            await NextAsync();
        }

        public void Dispose()
        {
            if (_isDisposed) return;

            _positionTimer?.Stop();
            _positionTimer?.Dispose();
            _audioPlayer?.Dispose();
            _sleepTimerCts?.Cancel();

            _isDisposed = true;
        }
    }
}
