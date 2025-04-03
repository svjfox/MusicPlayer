using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MusicPlayer.Models;
using MusicPlayer.Services;

namespace MusicPlayer.ViewModels
{
    public partial class PlayerViewModel : BaseViewModel
    {
        private readonly IAudioService _audioService;
        private readonly IEqualizerService _equalizerService;
        private readonly IDataService _dataService;

        [ObservableProperty]
        private Track _currentTrack;

        [ObservableProperty]
        private double _position;

        [ObservableProperty]
        private double _duration;

        [ObservableProperty]
        private string _playPauseIcon = "▶";

        [ObservableProperty]
        private bool _isShuffleEnabled;

        [ObservableProperty]
        private bool _isRepeatEnabled;

        [ObservableProperty]
        private double _volume = 0.8;

        [ObservableProperty]
        private double _playbackSpeed = 1.0;

        [ObservableProperty]
        private bool _isEqualizerOpen;

        [ObservableProperty]
        private string _selectedEqualizerPreset = "Flat";

        public PlayerViewModel(
            IAudioService audioService,
            IEqualizerService equalizerService,
            IDataService dataService)
        {
            _audioService = audioService;
            _equalizerService = equalizerService;
            _dataService = dataService;

            // Подписка на события
            _audioService.TrackChanged += OnTrackChanged;
            _audioService.PositionChanged += OnPositionChanged;
            _audioService.IsPlayingChanged += OnIsPlayingChanged;
            _audioService.ShuffleChanged += OnShuffleChanged;
            _audioService.RepeatChanged += OnRepeatChanged;
            _audioService.SleepTimerActivated += OnSleepTimerActivated;

            // Загрузка настроек
            LoadSettings();
        }

        private async void LoadSettings()
        {
            var volume = await SecureStorage.GetAsync("player_volume");
            if (double.TryParse(volume, out var vol))
            {
                Volume = vol;
                await _audioService.SetVolumeAsync(vol);
            }

            var speed = await SecureStorage.GetAsync("player_speed");
            if (double.TryParse(speed, out var spd))
            {
                PlaybackSpeed = spd;
                await _audioService.SetSpeedAsync(spd);
            }

            var shuffle = await SecureStorage.GetAsync("player_shuffle");
            if (bool.TryParse(shuffle, out var shf))
            {
                IsShuffleEnabled = shf;
                if (shf) await _audioService.ToggleShuffleAsync();
            }

            var repeat = await SecureStorage.GetAsync("player_repeat");
            if (bool.TryParse(repeat, out var rpt))
            {
                IsRepeatEnabled = rpt;
                if (rpt) await _audioService.ToggleRepeatAsync();
            }
        }

        private void OnTrackChanged(object sender, Track track)
        {
            MainThread.BeginInvokeOnMainThread(() =>
            {
                CurrentTrack = track;
                Duration = _audioService.Duration;

                // Инициализация эквалайзера
                if (_audioService.IsPlaying)
                {
                    _equalizerService.InitializeAsync(_audioService.GetPlayer());
                    _equalizerService.ApplyPreset(SelectedEqualizerPreset);
                }
            });
        }

        private void OnPositionChanged(object sender, double position)
        {
            MainThread.BeginInvokeOnMainThread(() =>
            {
                Position = position;
            });
        }

        private void OnIsPlayingChanged(object sender, bool isPlaying)
        {
            MainThread.BeginInvokeOnMainThread(() =>
            {
                PlayPauseIcon = isPlaying ? "⏸" : "▶";

                if (isPlaying)
                {
                    _equalizerService.InitializeAsync(_audioService.GetPlayer());
                    _equalizerService.ApplyPreset(SelectedEqualizerPreset);
                }
            });
        }

        private void OnShuffleChanged(object sender, bool isEnabled)
        {
            MainThread.BeginInvokeOnMainThread(() =>
            {
                IsShuffleEnabled = isEnabled;
            });
        }

        private void OnRepeatChanged(object sender, bool isEnabled)
        {
            MainThread.BeginInvokeOnMainThread(() =>
            {
                IsRepeatEnabled = isEnabled;
            });
        }

        private void OnSleepTimerActivated(object sender, bool isEnabled)
        {
            MainThread.BeginInvokeOnMainThread(async () =>
            {
                await Shell.Current.DisplayAlert("Sleep Timer", "Playback has been paused", "OK");
            });
        }

        [RelayCommand]
        private async Task TogglePlay()
        {
            if (_audioService.IsPlaying)
            {
                await _audioService.PauseAsync();
            }
            else
            {
                await _audioService.PlayAsync();
            }
        }

        [RelayCommand]
        private async Task Next()
        {
            await _audioService.NextAsync();
        }

        [RelayCommand]
        private async Task Previous()
        {
            await _audioService.PreviousAsync();
        }

        [RelayCommand]
        private async Task ToggleShuffle()
        {
            await _audioService.ToggleShuffleAsync();
        }

        [RelayCommand]
        private async Task ToggleRepeat()
        {
            await _audioService.ToggleRepeatAsync();
        }

        [RelayCommand]
        private async Task SeekTo(double position)
        {
            await _audioService.SeekToAsync(position);
        }

        [RelayCommand]
        private async Task VolumeChanged(double volume)
        {
            await _audioService.SetVolumeAsync(volume);
        }

        [RelayCommand]
        private async Task SpeedChanged(double speed)
        {
            await _audioService.SetSpeedAsync(speed);
        }

        [RelayCommand]
        private async Task OpenEqualizer()
        {
            IsEqualizerOpen = true;
            await _equalizerService.InitializeAsync(_audioService.GetPlayer());
            _equalizerService.ApplyPreset(SelectedEqualizerPreset);
        }

        [RelayCommand]
        private async Task CloseEqualizer()
        {
            IsEqualizerOpen = false;
        }

        [RelayCommand]
        private async Task ApplyEqualizerPreset(string preset)
        {
            SelectedEqualizerPreset = preset;
            _equalizerService.ApplyPreset(preset);
        }

        [RelayCommand]
        private async Task SetEqualizerBand(int bandIndex, float gain)
        {
            _equalizerService.SetBandGain(bandIndex, gain);
        }

        [RelayCommand]
        private async Task SetSleepTimer(int minutes)
        {
            await _audioService.SetSleepTimerAsync(minutes);
            await Shell.Current.DisplayAlert("Sleep Timer",
                $"Playback will stop in {minutes} minutes", "OK");
        }

        [RelayCommand]
        private async Task CancelSleepTimer()
        {
            await _audioService.CancelSleepTimer();
            await Shell.Current.DisplayAlert("Sleep Timer",
                "Sleep timer has been canceled", "OK");
        }

        public string TimeLeft => TimeSpan.FromSeconds(Duration - Position).ToString(@"mm\:ss");

        public List<string> EqualizerPresets => new() { "Flat", "Pop", "Rock", "Classical", "Jazz" };
        public List<double> SpeedOptions => new() { 0.5, 0.75, 1.0, 1.25, 1.5, 2.0 };
    }
}