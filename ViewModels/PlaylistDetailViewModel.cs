using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MusicPlayer.Models;
using MusicPlayer.Services;
using System.Collections.ObjectModel;

namespace MusicPlayer.ViewModels
{
    public partial class PlaylistDetailViewModel : BaseViewModel
    {
        private readonly IDataService _dataService;
        private readonly IAudioService _audioService;

        [ObservableProperty]
        private Playlist _playlist;

        [ObservableProperty]
        private ObservableCollection<Track> _tracks = new();

        public PlaylistDetailViewModel(IDataService dataService, IAudioService audioService)
        {
            _dataService = dataService;
            _audioService = audioService;
        }

        [RelayCommand]
        private async Task LoadTracks()
        {
            if (IsBusy || Playlist == null) return;

            try
            {
                IsBusy = true;
                var tracks = await _dataService.GetTracksForPlaylistAsync(Playlist.Id);
                Tracks = new ObservableCollection<Track>(tracks);
            }
            finally
            {
                IsBusy = false;
            }
        }

        [RelayCommand]
        private async Task PlayPlaylist()
        {
            if (Tracks.Count == 0) return;

            await _audioService.SetPlaylistAsync(Tracks);
            await _audioService.PlayAsync();
        }

        [RelayCommand]
        private async Task PlayTrack(Track track)
        {
            if (track == null) return;

            // Если текущий плейлист не установлен, устанавливаем его
            if (_audioService.CurrentPlaylist != Tracks)
            {
                await _audioService.SetPlaylistAsync(Tracks, Tracks.IndexOf(track));
            }
            else
            {
                await _audioService.LoadTrackAsync(track);
            }

            await _audioService.PlayAsync();
        }
    }
}