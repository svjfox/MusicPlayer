using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MusicPlayer.Models;

namespace MusicPlayer.Services
{
    public interface IAudioService : IDisposable
    {
        Task InitializeAsync();
        Task LoadTrackAsync(Track track);
        Task PlayAsync();
        Task PauseAsync();
        Task StopAsync();
        Task NextAsync();
        Task PreviousAsync();
        Task SeekToAsync(double position);
        Task SetVolumeAsync(double volume);
        Task SetSpeedAsync(double speed);
        Task ToggleShuffleAsync();
        Task ToggleRepeatAsync();
        Task SetPlaylistAsync(IEnumerable<Track> tracks, int startIndex = 0);
        Task SetSleepTimerAsync(int minutes);
        Task CancelSleepTimer();

        Task TogglePlay();

        bool IsPlaying { get; }
        bool IsShuffleEnabled { get; }
        bool IsRepeatEnabled { get; }
        bool IsSleepTimerActive { get; }
        double CurrentPosition { get; }
        double Duration { get; }
        Track CurrentTrack { get; }
        IReadOnlyList<Track> CurrentPlaylist { get; }

        event EventHandler<bool> IsPlayingChanged;
        event EventHandler<double> PositionChanged;
        event EventHandler<Track> TrackChanged;
        event EventHandler PlaybackEnded;
        event EventHandler<bool> ShuffleChanged;
        event EventHandler<bool> RepeatChanged;
        event EventHandler SleepTimerActivated;
    }
}