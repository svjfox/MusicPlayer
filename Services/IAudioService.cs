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

        bool IsPlaying { get; }
        double CurrentPosition { get; }
        double Duration { get; }
        Track CurrentTrack { get; }

        event EventHandler<bool> IsPlayingChanged;
        event EventHandler<double> PositionChanged;
        event EventHandler<Track> TrackChanged;
        event EventHandler PlaybackEnded;
    }
}