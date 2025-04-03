using Plugin.Maui.Audio;
using MusicPlayer.Models;
using System.Timers;
using MusicPlayer.Extensions;
using Timer = System.Timers.Timer;

namespace MusicPlayer.Services
{
    public class AudioService : IAudioService, IDisposable
    {
        private readonly IAudioManager _audioManager;
        private readonly IDataService _dataService;
        private IAudioPlayer _audioPlayer;
        private Timer _positionTimer;
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
        public IReadOnlyList<Track> Playlist => _playlist.AsReadOnly();
        public bool IsShuffleEnabled => _isShuffleEnabled;
        public bool IsRepeatEnabled => _isRepeatEnabled;

        public event EventHandler<bool> IsPlayingChanged;
        public event EventHandler<double> PositionChanged;
        public event EventHandler<Track> TrackChanged;
        public event EventHandler PlaybackEnded;
        public event EventHandler<bool> ShuffleChanged;
        public event EventHandler<bool> RepeatChanged;
        public event EventHandler SleepTimerActivated;

        public AudioService(IAudioManager audioManager, IDataService dataService)
        {
            _audioManager = audioManager;
            _dataService = dataService;
            _positionTimer = new Timer(500);
            _positionTimer.Elapsed += OnPositionTimerElapsed;
        }

        public async Task InitializeAsync()
        {
            if (_audioPlayer != null) return;

            _audioPlayer = _audioManager.CreatePlayer();
            _audioPlayer.PlaybackEnded += OnPlaybackEnded;
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
                // Остановить текущее воспроизведение
                if (_audioPlayer != null)
                {
                    _audioPlayer.Pause();
                    _audioPlayer.Dispose();
                }

                // Загрузить новый трек
                var stream = await FileSystem.OpenAppPackageFileAsync(track.Path);
                _audioPlayer = _audioManager.CreatePlayer(stream);
                _audioPlayer.PlaybackEnded += OnPlaybackEnded;

                // Обновить текущий трек
                CurrentTrack = track;
                TrackChanged?.Invoke(this, track);

                // Обновить уведомление (для Android)
                UpdateNotification();

                // Увеличить счетчик прослушиваний
                track.PlayCount++;
                await _dataService.UpdateTrackAsync(track);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading track: {ex.Message}");
                await Shell.Current.DisplayAlert("Error", "Failed to load track", "OK");
            }
        }

        public async Task PlayAsync()
        {
            if (_audioPlayer == null || CurrentTrack == null) return;

            await _audioPlayer.PlayAsync();
            IsPlaying = true;
            _positionTimer.Start();
            IsPlayingChanged?.Invoke(this, true);
            UpdateNotification();
        }

        public async Task PauseAsync()
        {
            if (_audioPlayer == null) return;

            await _audioPlayer.PauseAsync();
            IsPlaying = false;
            _positionTimer.Stop();
            IsPlayingChanged?.Invoke(this, false);
            UpdateNotification();
        }

        public async Task StopAsync()
        {
            if (_audioPlayer == null) return;

            await _audioPlayer.StopAsync();
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

            // Если трек играет меньше 3 секунд, перейти к предыдущему
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

            await _audioPlayer.SeekAsync(position);
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
            // Отменить предыдущий таймер
            _sleepTimerCts?.Cancel();

            if (minutes <= 0)
            {
                return;
            }

            _sleepTimerCts = new CancellationTokenSource();
            try
            {
                await Task.Delay(TimeSpan.FromMinutes(minutes), _sleepTimerCts.Token);
                await PauseAsync();
                SleepTimerActivated?.Invoke(this, true);
            }
            catch (TaskCanceledException)
            {
                // Таймер был отменен
            }
        }

        public async Task CancelSleepTimer()
        {
            _sleepTimerCts?.Cancel();
        }

        private void UpdateNotification()
        {
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
        }

        private void OnPositionTimerElapsed(object sender, ElapsedEventArgs e)
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