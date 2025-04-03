using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MusicPlayer.Models;
using MusicPlayer.Services;
using Plugin.Maui.Audio;

namespace MusicPlayer.ViewModels
{
    public partial class PlayerViewModel : BaseViewModel
    {
        private readonly IAudioService _audioService;
        private readonly IEqualizerService _equalizerService;

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

        public PlayerViewModel(IAudioService audioService, IEqualizerService equalizerService)
        {
            _audioService = audioService;
            _equalizerService = equalizerService;

            _audioService.TrackChanged += OnTrackChanged;
            _audioService.PositionChanged += OnPositionChanged;
            _audioService.IsPlayingChanged += OnIsPlayingChanged;
        }

        private void OnTrackChanged(object sender, Track track)
        {
            MainThread.BeginInvokeOnMainThread(() =>
            {
                CurrentTrack = track;
                Duration = _audioService.Duration;
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
            });
        }

        [RelayCommand]
        private async Task TogglePlay()
        {
            if (_audioService.IsPlaying)
                await _audioService.PauseAsync();
            else
                await _audioService.PlayAsync();
        }

        [RelayCommand]
        private async Task Next() => await _audioService.NextAsync();

        [RelayCommand]
        private async Task Previous() => await _audioService.PreviousAsync();

        [RelayCommand]
        private async Task ToggleShuffle() => await _audioService.ToggleShuffleAsync();

        [RelayCommand]
        private async Task ToggleRepeat() => await _audioService.ToggleRepeatAsync();

        [RelayCommand]
        private async Task SeekTo(double position) => await _audioService.SeekToAsync(position);

        [RelayCommand]
        private async Task VolumeChanged(double volume) => await _audioService.SetVolumeAsync(volume);

        [RelayCommand]
        private async Task SpeedChanged(double speed) => await _audioService.SetSpeedAsync(speed);

        [RelayCommand]
        private async Task ShowSleepTimer()
        {
            var result = await Shell.Current.DisplayActionSheet(
                "Sleep Timer", "Cancel", null,
                "Off", "5 minutes", "10 minutes", "30 minutes", "1 hour");

            if (result == "Off")
            {
                await _audioService.CancelSleepTimer();
                return;
            }

            var minutes = result switch
            {
                "5 minutes" => 5,
                "10 minutes" => 10,
                "30 minutes" => 30,
                "1 hour" => 60,
                _ => 0
            };

            if (minutes > 0)
            {
                await _audioService.SetSleepTimerAsync(minutes);
                await Shell.Current.DisplayAlert("Sleep Timer",
                    $"Playback will stop in {minutes} minutes", "OK");
            }
        }

        public string TimeLeft => TimeSpan.FromSeconds(Duration - Position).ToString(@"mm\:ss");
    }
}